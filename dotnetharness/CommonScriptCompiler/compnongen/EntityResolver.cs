using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class EntityResolverUtil
    {
        public static void EntityResolver_ResetAutoVarId(Resolver resolver)
        {
            resolver.autoVarId = 0;
        }

        public static int EntityResolver_GetNextAutoVarId(Resolver resolver)
        {
            return resolver.autoVarId++;
        }

        public static void EntityResolver_DetermineMemberOffsets(ClassEntity classDef)
        {
            // TODO: you need to ensure that the overridden members are exclusively methods, not fields.

            if (classDef.directMemberToOffset != null) return;
            ClassEntity parent = classDef.baseClassEntity;
            if (parent != null) EntityResolver_DetermineMemberOffsets(parent);

            classDef.directMemberToOffset = new Dictionary<string, int>();
            classDef.flattenedMemberOffsetLookup = new Dictionary<string, int>();
            List<string> newDirectMembers = new List<string>();
            List<string> staticFieldNames = new List<string>();
            List<string> staticMethodNames = new List<string>();

            if (parent != null)
            {
                foreach (string parentKey in parent.flattenedMemberOffsetLookup.Keys)
                {
                    classDef.flattenedMemberOffsetLookup[parentKey] = parent.flattenedMemberOffsetLookup[parentKey];
                }
            }

            int nextOffset = classDef.flattenedMemberOffsetLookup.Count;
            string[] memberNames = classDef.classMembers.Keys.OrderBy(k => k).ToArray();
            for (int i = 0; i < memberNames.Length; i++)
            {
                string memberName = memberNames[i];
                AbstractEntity member = classDef.classMembers[memberName];

                if (!member.isStatic &&
                   (member.type == (int)EntityType.FIELD || member.type == (int)EntityType.FUNCTION))
                {
                    int offset = 0;
                    if (!classDef.flattenedMemberOffsetLookup.ContainsKey(memberName))
                    {
                        offset = nextOffset;
                        nextOffset++;
                        newDirectMembers.Add(memberName);
                        classDef.directMemberToOffset[memberName] = offset;
                        classDef.flattenedMemberOffsetLookup[memberName] = offset;
                    }
                }
            }

            classDef.newDirectMemberOffsets = newDirectMembers.ToArray();
        }

        public static void EntityResolver_ResolveFunctionFirstPass(Resolver resolver, FunctionEntity funcDef)
        {
            funcDef.variableScope = new Dictionary<string, bool>();

            resolver.activeEntity = funcDef.baseData;
            resolver.breakContext = null;

            for (int i = 0; i < funcDef.argTokens.Length; i++)
            {
                Token arg = funcDef.argTokens[i];
                if (funcDef.variableScope.ContainsKey(arg.Value))
                {
                    FunctionWrapper.Errors_Throw(arg, "There are multiple arguments named '" + arg.Value + "'.");
                }
                funcDef.variableScope[arg.Value] = true;

                Expression defVal = funcDef.argDefaultValues[i];
                if (defVal != null) funcDef.argDefaultValues[i] = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, defVal);
            }

            List<Statement> preBaseFieldInit = new List<Statement>();
            List<Statement> postBaseFieldInit = new List<Statement>();
            List<Statement> baseCtorInvocation = new List<Statement>();

            bool isCtor = funcDef.baseData.type == (int)EntityType.CONSTRUCTOR;
            FunctionEntity ctorEnt = null;

            if (isCtor)
            {
                ctorEnt = funcDef;

                Dictionary<string, AbstractEntity> siblings = FunctionWrapper.Entity_getMemberLookup(resolver.staticCtx, funcDef.baseData.nestParent);
                // TODO: this is wrong. The fields should be processed in the order that they appear in the code itself.
                // This will affect runtime order of complex expressions which could have observable consequences.
                // TODO: simple constant initial values should be conveyed in the metadata and direct-copied from a template.
                // Ctrl+F for initialValues in the runtime.

                FieldEntity[] fields = siblings.Keys
                    .OrderBy(k => k)
                    .Select(k => siblings[k])
                    .Where(e => e.type == (int)EntityType.FIELD && e.isStatic == ctorEnt.baseData.isStatic)
                    .Select(e => (FieldEntity)e.specificData)
                    .Where(e => e.defaultValue != null)
                    .ToArray();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldEntity fld = fields[i];
                    fld.defaultValue = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionFirstPass(resolver, fld.defaultValue);
                    Statement setter = EntityResolver_ConvertFieldDefaultValueIntoSetter(fld);
                    if (ResolverUtil.IsExpressionConstant(fld.defaultValue))
                    {
                        preBaseFieldInit.Add(setter);
                    }
                    else
                    {
                        postBaseFieldInit.Add(setter);
                    }
                }
            }

            if (isCtor && 
                funcDef.baseData.nestParent.type == (int)EntityType.CLASS && 
                ((ClassEntity) funcDef.baseData.nestParent.specificData).baseClassEntity != null)
            {
                // TODO: verify arg count
                Token baseCtor = funcDef.baseData.firstToken; // TODO: this is wrong, get the actual 'base' token
                Token baseCtorParen = funcDef.baseData.firstToken; // TODO: this is even more wrong 
                Expression baseCtorRef = FunctionWrapper.Expression_createBaseCtorReference(baseCtor);
                Expression baseCtorInvoke = FunctionWrapper.Expression_createFunctionInvocation(baseCtorRef, baseCtorParen, ctorEnt.baseCtorArgValues);
                Statement baseCtorStmnt = FunctionWrapper.Statement_createExpressionAsStatement(baseCtorInvoke);
                baseCtorStmnt = StatementResolverUtil.StatementResolver_ResolveStatementFirstPass(resolver, baseCtorStmnt);
                baseCtorInvocation.Add(baseCtorStmnt);
            }

            StatementResolverUtil.StatementResolver_ResolveStatementArrayFirstPass(resolver, funcDef.code);

            List<Statement> flattened = new List<Statement>();
            for (int i = 0; i < preBaseFieldInit.Count; i++) flattened.Add(preBaseFieldInit[i]);
            for (int i = 0; i < baseCtorInvocation.Count; i++) flattened.Add(baseCtorInvocation[i]);
            for (int i = 0; i < postBaseFieldInit.Count; i++) flattened.Add(postBaseFieldInit[i]);
            for (int i = 0; i < funcDef.code.Length; i++) flattened.Add(funcDef.code[i]);

            Statement lastStatement = null;
            if (flattened.Count > 0) lastStatement = flattened[flattened.Count - 1];
            
            bool autoReturnNeeded = lastStatement == null ||
                (lastStatement.type != (int) StatementType.RETURN && lastStatement.type != (int) StatementType.THROW);
            if (autoReturnNeeded)
            {
                flattened.Add(FunctionWrapper.Statement_createReturn(null, FunctionWrapper.Expression_createNullConstant(null)));
            }
            funcDef.code = [.. flattened];

            resolver.activeEntity = null;
            resolver.breakContext = null;
        }

        private static Statement EntityResolver_ConvertFieldDefaultValueIntoSetter(FieldEntity fld)
        {
            if (fld.opToken == null) throw new InvalidOperationException(); // only applicable to default-value-based fields.
            Expression root = null;
            if (fld.baseData.isStatic)
            {
                root = FunctionWrapper.Expression_createClassReference(null, fld.baseData.nestParent);
            }
            else
            {
                root = FunctionWrapper.Expression_createThisReference(null);
            }
            Expression target = FunctionWrapper.Expression_createDotField(root, null, fld.baseData.simpleName);
            Token equal = fld.opToken;
            return FunctionWrapper.Statement_createAssignment(target, equal, fld.defaultValue);
        }

        public static void EntityResolver_ResolveFunctionSecondPass(Resolver resolver, FunctionEntity funcDef)
        {
            resolver.activeEntity = funcDef.baseData;
            resolver.breakContext = null;

            for (int i = 0; i < funcDef.argDefaultValues.Length; i++)
            {
                Expression defVal = funcDef.argDefaultValues[i];
                if (defVal != null)
                {
                    funcDef.argDefaultValues[i] = ExpressionResolverUtil.ExpressionResolver_ResolveExpressionSecondPass(resolver, defVal);
                }
            }

            StatementResolverUtil.StatementResolver_ResolveStatementArraySecondPass(resolver, funcDef.code);

            if (funcDef.code.Length == 0 || funcDef.code[funcDef.code.Length - 1].type != (int) StatementType.RETURN)
            {
                List<Statement> newCode = new List<Statement>(funcDef.code);
                newCode.Add(FunctionWrapper.Statement_createReturn(null, FunctionWrapper.Expression_createNullConstant(null)));
                funcDef.code = newCode.ToArray();
            }
            resolver.activeEntity = null;
            resolver.breakContext = null;
        }
    }
}
