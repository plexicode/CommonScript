using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class EntityResolver
    {
        private Resolver resolver;
        public ExpressionResolver expressionResolver;
        public StatementResolver statementResolver;

        public EntityResolver(Resolver resolver)
        {
            this.resolver = resolver;
        }

        public static void DetermineMemberOffsets(ClassEntity classDef)
        {
            // TODO: you need to ensure that the overridden members are exclusively methods, not fields.

            if (classDef.directMemberToOffset != null) return;
            ClassEntity parent = classDef.baseClassEntity;
            if (parent != null) DetermineMemberOffsets(parent);

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
                   (member.type == EntityType.FIELD || member.type == EntityType.FUNCTION))
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

        public void ResolveFunctionFirstPass(FunctionLikeEntity funcDef)
        {
            funcDef.variableScope = new Dictionary<string, bool>();

            this.resolver.activeEntity = funcDef;
            this.resolver.breakContext = null;

            for (int i = 0; i < funcDef.argTokens.Length; i++)
            {
                Token arg = funcDef.argTokens[i];
                if (funcDef.variableScope.ContainsKey(arg.Value))
                {
                    Errors.ThrowError(arg, "There are multiple arguments named '" + arg.Value + "'.");
                }
                funcDef.variableScope[arg.Value] = true;

                Expression defVal = funcDef.argDefaultValues[i];
                if (defVal != null) funcDef.argDefaultValues[i] = this.expressionResolver.ResolveExpressionFirstPass(defVal);
            }

            List<Statement> preBaseFieldInit = new List<Statement>();
            List<Statement> postBaseFieldInit = new List<Statement>();
            List<Statement> baseCtorInvocation = new List<Statement>();

            bool isCtor = funcDef.type == EntityType.CONSTRUCTOR;
            ConstructorEntity ctorEnt = null;

            if (isCtor)
            {
                ctorEnt = (ConstructorEntity)funcDef;

                Dictionary<string, AbstractEntity> siblings = funcDef.nestParent.getMemberLookup();
                // TODO: this is wrong. The fields should be processed in the order that they appear in the code itself.
                // This will affect runtime order of complex expressions which could have observable consequences.
                // TODO: simple constant initial values should be conveyed in the metadata and direct-copied from a template.
                // Ctrl+F for initialVaues in the runtime.

                FieldEntity[] fields = siblings.Keys
                    .OrderBy(k => k)
                    .Select(k => siblings[k])
                    .Where(e => e.type == EntityType.FIELD && e.isStatic == ctorEnt.isStatic)
                    .Cast<FieldEntity>()
                    .Where(e => e.defaultValue != null)
                    .ToArray();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldEntity fld = fields[i];
                    fld.defaultValue = this.expressionResolver.ResolveExpressionFirstPass(fld.defaultValue);
                    Statement setter = ConvertFieldDefaultValueIntoSetter(fld);
                    if (Resolver.IsExpressionConstant(fld.defaultValue))
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
                funcDef.nestParent.type == EntityType.CLASS && 
                ((ClassEntity) funcDef.nestParent).baseClassEntity != null)
            {
                // TODO: verify arg count
                Token baseCtor = funcDef.firstToken; // TODO: this is wrong, get the actual 'base' token
                Token baseCtorParen = funcDef.firstToken; // TODO: this is even more wrong 
                Expression baseCtorRef = Expression.createBaseCtorReference(baseCtor);
                Expression baseCtorInvoke = Expression.createFunctionInvocation(baseCtorRef, baseCtorParen, ctorEnt.baseCtorArgValues);
                Statement baseCtorStmnt = Statement.createExpressionAsStatement(baseCtorInvoke);
                baseCtorStmnt = this.statementResolver.ResolveStatementFirstPass(baseCtorStmnt);
                baseCtorInvocation.Add(baseCtorStmnt);
            }

            this.statementResolver.ResolveStatementArrayFirstPass(funcDef.code);

            List<Statement> flattened = [
                .. preBaseFieldInit,
                .. baseCtorInvocation,
                .. postBaseFieldInit,
                .. funcDef.code,
            ];

            Statement lastStatement = flattened.Count == 0 ? null : flattened[flattened.Count - 1];
            bool autoReturnNeeded = lastStatement == null ||
                (lastStatement.type != StatementType.RETURN && lastStatement.type != StatementType.THROW);
            if (autoReturnNeeded)
            {
                flattened.Add(Statement.createReturn(null, Expression.createNullConstant(null)));
            }
            funcDef.code = [.. flattened];

            this.resolver.activeEntity = null;
            this.resolver.breakContext = null;
        }

        private static Statement ConvertFieldDefaultValueIntoSetter(FieldEntity fld)
        {
            if (fld.opToken == null) throw new InvalidOperationException(); // only applicable to default-value-based fields.
            Expression root = fld.isStatic
                ? Expression.createClassReference(null, fld.nestParent)
                : Expression.createThisReference(null);
            Expression target = Expression.createDotField(root, null, fld.simpleName);
            Token equal = fld.opToken;
            return Statement.createAssignment(target, equal, fld.defaultValue);
        }

        public void ResolveFunctionSecondPass(FunctionLikeEntity funcDef)
        {
            this.resolver.activeEntity = funcDef;
            this.resolver.breakContext = null;

            for (int i = 0; i < funcDef.argDefaultValues.Length; i++)
            {
                Expression defVal = funcDef.argDefaultValues[i];
                if (defVal != null)
                {
                    funcDef.argDefaultValues[i] = this.expressionResolver.ResolveExpressionSecondPass(defVal);
                }
            }

            this.statementResolver.ResolveStatementArraySecondPass(funcDef.code);

            if (funcDef.code.Length == 0 || funcDef.code[funcDef.code.Length - 1].type != StatementType.RETURN)
            {
                List<Statement> newCode = new List<Statement>(funcDef.code);
                newCode.Add(Statement.createReturn(null, Expression.createNullConstant(null)));
                funcDef.code = newCode.ToArray();
            }
            this.resolver.activeEntity = null;
            this.resolver.breakContext = null;
        }
    }
}
