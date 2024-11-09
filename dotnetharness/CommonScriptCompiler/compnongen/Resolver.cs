using System;
using System.Collections.Generic;
using System.Linq;

namespace CommonScript.Compiler
{
    internal class Resolver
    {
        public Dictionary<string, AbstractEntity> nestedEntities;
        public Dictionary<string, AbstractEntity> enumsByMemberFqName;
        public Dictionary<string, AbstractEntity> builtinRefs;
        public Dictionary<string, AbstractEntity> flattenedEntities;

        public AbstractEntity activeEntity = null;
        public AbstractEntity[] entityList = null;
        public Statement breakContext = null;

        private ExpressionResolver expressionResolver;
        private StatementResolver statementResolver;
        private EntityResolver entityResolver;

        private List<LambdaEntity> lambdas = new List<LambdaEntity>();
        private HashSet<string> extensionNames = new HashSet<string>();

        public Resolver(Dictionary<string, AbstractEntity> rootEntities, ICollection<string> extensionNames)
        {
            this.extensionNames = new HashSet<string>(extensionNames);

            this.expressionResolver = new ExpressionResolver(this);
            this.statementResolver = new StatementResolver(this);
            this.entityResolver = new EntityResolver(this);
            this.expressionResolver.statementResolver = this.statementResolver;
            this.entityResolver.statementResolver = this.statementResolver;
            this.expressionResolver.entityResolver = this.entityResolver;
            this.statementResolver.entityResolver = this.entityResolver;
            this.statementResolver.expressionResolver = this.expressionResolver;
            this.entityResolver.expressionResolver = this.expressionResolver;

            this.nestedEntities = rootEntities;
            this.flattenedEntities = new Dictionary<string, AbstractEntity>();
            this.enumsByMemberFqName = new Dictionary<string, AbstractEntity>();

            this.entityList = Resolver.FlattenEntities(rootEntities);
            foreach (AbstractEntity tle in this.entityList)
            {
                this.flattenedEntities[tle.fqName] = tle;
            }

            foreach (EnumEntity enumDef in this.entityList.OfType<EnumEntity>())
            {
                for (int i = 0; i < enumDef.memberNameTokens.Length; i++)
                {
                    string fqName = enumDef.fqName + "." + enumDef.memberNameTokens[i].Value;
                    this.enumsByMemberFqName[fqName] = enumDef;
                }
            }
        }

        public bool isValidRegisteredExtension(string extensionName)
        {
            return this.extensionNames.Contains(extensionName);
        }

        internal static AbstractEntity[] FlattenEntities(Dictionary<string, AbstractEntity> rootEntities)
        {
            List<AbstractEntity> output = new List<AbstractEntity>();
            List<AbstractEntity> queue = new List<AbstractEntity>(rootEntities.Values);
            for (int i = 0; i < queue.Count; i++)
            {
                AbstractEntity entity = queue[i];
                output.Add(entity);

                foreach (AbstractEntity mem in entity.getMemberLookup().Values)
                {
                    queue.Add(mem);
                }
            }
            return output.ToArray();
        }

        public void Resolve()
        {
            List<FunctionEntity> functions = new List<FunctionEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<ConstEntity> constants = new List<ConstEntity>();
            List<EnumEntity> enums = new List<EnumEntity>();
            List<ConstructorEntity> constructors = new List<ConstructorEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();

            AbstractEntity[] entities = this.flattenedEntities.Values.ToArray();

            for (int i = 0; i < entities.Length; i++)
            {
                AbstractEntity tle = entities[i];

                if (tle.type == EntityType.CONST)
                {
                    constants.Add((ConstEntity)tle);
                }
                else if (tle.type == EntityType.ENUM)
                {
                    enums.Add((EnumEntity)tle);
                }
                else if (tle.type == EntityType.FUNCTION)
                {
                    functions.Add((FunctionEntity)tle);
                }
                else if (tle.type == EntityType.CLASS)
                {
                    classes.Add((ClassEntity)tle);
                }
                else if (tle.type == EntityType.CONSTRUCTOR)
                {
                    constructors.Add((ConstructorEntity)tle);
                }
                else if (tle.type == EntityType.FIELD)
                {
                    fields.Add((FieldEntity)tle);
                }
                else if (tle.type == EntityType.NAMESPACE)
                {
                    // Nothing to resolve.
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            this.AddImplicitIncrementingEnumValueDefinitions(enums);

            string[] constAndEnumResolutionOrder = this.DetermineConstAndEnumResolutionOrder(constants, enums);

            this.PerformFullResolutionPassOnConstAndEnums(constAndEnumResolutionOrder);

            ClassEntity[] orderedClasses = this.ResolveBaseClassesAndEstablishClassOrder(classes, this.flattenedEntities);

            for (int i = 0; i < orderedClasses.Length; i++)
            {
                EntityResolver.DetermineMemberOffsets(orderedClasses[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                this.entityResolver.ResolveFunctionFirstPass(functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                this.entityResolver.ResolveFunctionFirstPass(constructors[i]);
            }

            // At this point, all lambdas have been repoerted.
            for (int i = 0; i < lambdas.Count; i++) // DO NOT CHANGE TO FOR-EACH. This count can grow as a result of nested lambdas.
            {
                this.entityResolver.ResolveFunctionFirstPass(lambdas[i]);
            }

            for (int i = 0; i < functions.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(functions[i]);
            }

            for (int i = 0; i < constructors.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(constructors[i]);
            }

            for (int i = 0; i < lambdas.Count; i++)
            {
                this.entityResolver.ResolveFunctionSecondPass(lambdas[i]);
            }
        }

        internal void ReportNewLambda(LambdaEntity lamb)
        {
            this.lambdas.Add(lamb);
        }

        private ClassEntity[] ResolveBaseClassesAndEstablishClassOrder(
            List<ClassEntity> classes,
            Dictionary<string, AbstractEntity> flattenedEntities)
        {
            // TODO: add external module resolving as a separate feature.
            ClassEntity[] deterministicOrder = classes.OrderBy(c => c.fqName).ToArray();
            List<ClassEntity> finalOrder = new List<ClassEntity>();
            List<ClassEntity> baseClassRequired = new List<ClassEntity>();
            foreach (ClassEntity e in deterministicOrder)
            {
                if (e.baseClassTokens != null)
                {
                    baseClassRequired.Add(e);
                }
                else
                {
                    finalOrder.Add(e);
                }
            }
            foreach (ClassEntity bc in baseClassRequired)
            {
                string ns = string.Join('.', bc.fqName.Split('.').SkipLast(1));
                this.activeEntity = bc;
                Token baseClassToken = bc.baseClassTokens[0];
                AbstractEntity bcEntity = this.DoLookup2(baseClassToken, baseClassToken.Value);
                if (bcEntity != null)
                {
                    for (int i = 2; i < bc.baseClassTokens.Length; i += 2)
                    {
                        string next = bc.baseClassTokens[i].Value;
                        if (bcEntity != null &&
                            bcEntity.getMemberLookup().ContainsKey(next))
                        {
                            bcEntity = bcEntity.getMemberLookup()[next];
                        }
                        else
                        {
                            bcEntity = null;
                        }
                    }
                }
                if (bcEntity == null)
                {
                    Errors.ThrowError(bc.firstToken, "Could not resolve base class");
                    throw new NotImplementedException();
                }
                if (bcEntity.type != EntityType.CLASS)
                {
                    Errors.ThrowError(bc.firstToken, bcEntity.fqName + " is not a valid class.");
                }

                bc.baseClassEntity = (ClassEntity)bcEntity;
                this.activeEntity = null;
            }

            Dictionary<string, bool> includedInOrder = new Dictionary<string, bool>();
            foreach (AbstractEntity bc in baseClassRequired)
            {
                includedInOrder[bc.fqName] = false;
            }
            foreach (ClassEntity bc in baseClassRequired)
            {
                ClassEntity walker = bc;
                List<ClassEntity> order = new List<ClassEntity>();
                while (walker != null)
                {
                    // If it's external or already included, stop.
                    if (!includedInOrder.ContainsKey(walker.fqName) ||
                        includedInOrder[walker.fqName])
                    {
                        walker = null;
                    }
                    else
                    {
                        order.Add(walker);
                        includedInOrder[walker.fqName] = true;
                        walker = walker.baseClassEntity;
                    }

                    if (order.Count > deterministicOrder.Length)
                    {
                        Errors.ThrowError(bc.firstToken, "This class has a cycle in its base class chain.");
                    }
                }

                order.Reverse();
                finalOrder.AddRange(order);
            }

            return finalOrder.ToArray();
        }

        private void PerformFullResolutionPassOnConstAndEnums(string[] resOrder)
        {
            for (int passNum = 1; passNum <= 2; passNum++)
            {
                for (int i = 0; i < resOrder.Length; i++)
                {
                    AbstractEntity entity = this.DoLookup("", resOrder[i]);
                    this.activeEntity = entity;
                    if (entity.type == EntityType.ENUM)
                    {
                        EnumEntity enumDef = (EnumEntity)entity;
                        int memberIndex = this.GetEnumMemberIndex(resOrder[i], enumDef);
                        Expression val = enumDef.memberValues[memberIndex];
                        if (passNum == 1)
                        {
                            val = this.expressionResolver.ResolveExpressionFirstPass(val);
                        }
                        else
                        {
                            val = this.expressionResolver.ResolveExpressionSecondPass(val);
                        }

                        if (passNum == 2 && val.type != ExpressionType.INTEGER_CONST)
                        {
                            Errors.ThrowError(enumDef.memberNameTokens[memberIndex], "This enum value has a non-integer value.");
                        }

                        enumDef.memberValues[memberIndex] = val;
                    }
                    else
                    {
                        ConstEntity constEnt = (ConstEntity)entity;
                        Expression val = constEnt.constValue;
                        if (passNum == 1)
                        {
                            val = this.expressionResolver.ResolveExpressionFirstPass(val);
                        }
                        else
                        {
                            val = this.expressionResolver.ResolveExpressionSecondPass(val);
                            if (!IsExpressionConstant(val))
                            {
                                Errors.ThrowError(val.firstToken, "A constant expression is required here.");
                            }
                        }

                        constEnt.constValue = val;
                    }
                    this.activeEntity = null;
                }
            }
        }

        internal static bool IsExpressionConstant(Expression expr)
        {
            switch (expr.type)
            {
                case ExpressionType.BOOL_CONST:
                case ExpressionType.NULL_CONST:
                case ExpressionType.INTEGER_CONST:
                case ExpressionType.FLOAT_CONST:
                case ExpressionType.STRING_CONST:
                case ExpressionType.ENUM_CONST:
                    return true;
            }
            return false;
        }


        private static Token BuildFakeToken(Token template, string value, bool isPunc)
        {
            return new Token(
                value,
                isPunc ? TokenType.PUNCTUATION : TokenType.NAME,
                template.File,
                template.Line,
                template.Col);
        }

        private static Expression BuildFakeDotChain(string root, string field)
        {
            Expression varRoot = Expression.createVariable(null, root);
            return Expression.createDotField(varRoot, null, field);
        }

        // For all undefined values of an enum, make it equal to the previous value + 1 by
        // injecting EnumName.MemberName + 1
        // If the first value is undefined, inject 1
        private void AddImplicitIncrementingEnumValueDefinitions(List<EnumEntity> enums)
        {
            for (int i = 0; i < enums.Count; i++)
            {
                EnumEntity enumEnt = enums[i];
                for (int j = 0; j < enumEnt.memberNameTokens.Length; j++)
                {
                    Token token = enumEnt.memberNameTokens[j];
                    if (enumEnt.memberValues[j] == null)
                    {
                        if (j == 0)
                        {
                            enumEnt.memberValues[j] = Expression.createIntegerConstant(token, 1);
                        }
                        else
                        {
                            enumEnt.memberValues[j] = Expression.createBinaryOp(
                                BuildFakeDotChain(enumEnt.simpleName, enumEnt.memberNameTokens[j - 1].Value),
                                BuildFakeToken(token, "+", true),
                                Expression.createIntegerConstant(null, 1)
                            );
                        }
                    }
                }
            }
        }

        // Returns strings because we treat the individual enum values as separate entities.
        private string[] DetermineConstAndEnumResolutionOrder(
            List<ConstEntity> constants,
            List<EnumEntity> enums)
        {
            Dictionary<string, List<string>> referencesMadeByFqItem = new Dictionary<string, List<string>>();

            // You left off thinking about how to register references found in the expression resolver
            // out here and then using that to build up the references lookup.
            // Then you were thinking about how this only pertains to the first pass, that really this 
            // function should do both passes and be renamed to ResolveConstantsAndEnums()


            foreach (ConstEntity c in constants)
            {
                string ns = c.nestParent == null ? "" : c.nestParent.fqName;

                referencesMadeByFqItem[c.fqName] = this.GetListOfConstReferences(ns, c.constValue);
            }

            foreach (EnumEntity e in enums)
            {
                string ns = e.nestParent == null ? "" : e.nestParent.fqName;
                int memCount = e.memberNameTokens.Length;

                for (int i = 0; i < memCount; i++)
                {
                    string memFqName = e.fqName + "." + e.memberNameTokens[i].Value;
                    Expression val = e.memberValues[i];
                    referencesMadeByFqItem[memFqName] = this.GetListOfConstReferences(ns, val);
                }
            }

            // 0 - unresolved, 1 - resolving, 2 - resolved
            Dictionary<string, int> resolutionStatus = new Dictionary<string, int>();
            List<string> resolutionOrder = new List<string>();

            List<string> q = new List<string>(referencesMadeByFqItem.Keys.OrderBy(k => k).Reverse());
            while (q.Count > 0)
            {
                int lastIndex = q.Count - 1;
                string itemFqName = q[lastIndex];
                q.RemoveAt(lastIndex);
                if (itemFqName == "*")
                {
                    // indicator that the next item has all prerequisites added to the list.
                    // it is now safe to add it to the list.
                    string resolvedItem = q[lastIndex - 1];
                    q.RemoveAt(lastIndex - 1);
                    resolutionStatus[resolvedItem] = 2;
                    resolutionOrder.Add(resolvedItem);
                }
                else
                {
                    AbstractEntity item = this.DoLookup("", itemFqName);
                    Token itemToken = item.firstToken;
                    if (item.fqName != itemFqName) // an enum member
                    {
                        EnumEntity parentEnum = (EnumEntity) item;
                        int enumValIndex = this.GetEnumMemberIndex(itemFqName, parentEnum);
                        itemToken = parentEnum.memberNameTokens[enumValIndex];
                    }

                    int status = 0;
                    if (resolutionStatus.ContainsKey(itemFqName)) status = resolutionStatus[itemFqName];

                    if (status == 1)
                    {
                        Errors.ThrowError(item.firstToken, "This definition contains a resolution cycle.");
                    }

                    if (status == 2)
                    {
                        // Already resolved. Skip!
                    }
                    else
                    {
                        q.Add(itemFqName);
                        q.Add("*");
                        resolutionStatus[itemFqName] = 1;
                        List<string> references = referencesMadeByFqItem[itemFqName];
                        for (int i = references.Count - 1; i >= 0; i--)
                        {
                            q.Add(references[i]);
                        }
                    }
                }
            }

            return resolutionOrder.ToArray();
        }

        private int GetEnumMemberIndex(string memNameOrFqMemName, EnumEntity enumEntity)
        {
            int lastDot = memNameOrFqMemName.LastIndexOf('.');
            string memName = memNameOrFqMemName;
            if (lastDot != -1)
            {
                memName = memNameOrFqMemName.Substring(lastDot + 1);
            }

            for (int i = 0; i < enumEntity.memberNameTokens.Length; i++)
            {
                if (enumEntity.memberNameTokens[i].Value == memName)
                {
                    return i;
                }
            }

            return -1;
        }

        private List<string> GetListOfConstReferences(string fqNamespace, Expression expr)
        {
            List<string> refs = new List<string>();
            this.GetListOfConstReferencesImpl(fqNamespace, expr, refs);
            return refs;
        }

        private void GetListOfConstReferencesImpl(string fqNamespace, Expression expr, List<string> refs)
        {
            switch (expr.type)
            {
                case ExpressionType.INTEGER_CONST:
                case ExpressionType.BOOL_CONST:
                case ExpressionType.FLOAT_CONST:
                case ExpressionType.NULL_CONST:
                case ExpressionType.STRING_CONST:
                    // This is fine
                    break;

                case ExpressionType.BINARY_OP:
                    this.GetListOfConstReferencesImpl(fqNamespace, expr.left, refs);
                    this.GetListOfConstReferencesImpl(fqNamespace, expr.right, refs);
                    break;

                case ExpressionType.VARIABLE:
                    AbstractEntity referenced = this.DoLookup(fqNamespace, expr.strVal);
                    if (referenced == null)
                    {
                        Errors.ThrowError(expr.firstToken, "No definition for '" + expr.strVal + "'");
                    }
                    else
                    {
                        switch (referenced.type)
                        {
                            case EntityType.CONST:
                                refs.Add(referenced.fqName);
                                break;
                            case EntityType.ENUM:
                                throw new NotImplementedException();

                            default:
                                Errors.ThrowError(expr.firstToken, "Cannot refer to this entity from a constant expression.");
                                break;
                        }
                    }
                    break;

                case ExpressionType.DOT_FIELD:
                    // Walk the dot chain down to the variable root and use this as a lookup.
                    List<string> dotFieldChainBuilder = new List<string>();
                    Expression walker = expr;
                    while (walker.type == ExpressionType.DOT_FIELD)
                    {
                        dotFieldChainBuilder.Add(walker.strVal);
                        walker = walker.root;
                    }
                    if (walker.type != ExpressionType.VARIABLE)
                    {
                        Errors.ThrowError(walker.firstToken, "Cannot use this type of entity from a constant expression.");
                    }
                    dotFieldChainBuilder.Add(walker.strVal);
                    dotFieldChainBuilder.Reverse();
                    string dotFieldChain = string.Join('.', dotFieldChainBuilder);
                    AbstractEntity reffedEntity = this.DoLookup(fqNamespace, dotFieldChain);
                    if (reffedEntity == null) Errors.ThrowError(expr.firstToken, "Invalid expression for constant.");
                    if (reffedEntity.type == EntityType.CONST)
                    {
                        refs.Add(reffedEntity.fqName);
                    }
                    else if (reffedEntity.type == EntityType.ENUM)
                    {
                        string enumMemberName = dotFieldChainBuilder[dotFieldChainBuilder.Count - 1];
                        string enumName = dotFieldChain.Substring(0, dotFieldChain.Length - enumMemberName.Length - 1);
                        AbstractEntity enumParentCheck = this.DoLookup(fqNamespace, enumName);
                        if (enumParentCheck != reffedEntity)
                        {
                            Errors.ThrowError(expr.firstToken, "Cannot refer to enum types directly in constant expression. You can only reference an enum member.");
                        }

                        refs.Add(enumParentCheck.fqName + "." + enumMemberName);
                    }
                    else
                    {
                        Errors.ThrowError(expr.firstToken, "Cannot reference this entity from here.");
                    }
                    break;

                default:
                    Errors.ThrowError(expr.firstToken, "Invalid expression for constant.");
                    break;
            }
        }

        private AbstractEntity DoDirectLookup(string fqAttempt)
        {
            if (this.flattenedEntities.ContainsKey(fqAttempt))
            {
                return this.flattenedEntities[fqAttempt];
            }
            return null;
        }

        internal AbstractEntity DoLookup2(Token throwToken, string name)
        {
            // root level entities and namespaces always have precedence.
            if (this.flattenedEntities.ContainsKey(name))
            {
                return this.flattenedEntities[name];
            }

            if (this.activeEntity.fileContext.importsByVar.ContainsKey(name))
            {
                return new ModuleWrapperEntity(throwToken, this.activeEntity.fileContext.importsByVar[name]);
            }

            AbstractEntity walker = this.activeEntity;
            while (walker != null)
            {
                if (walker.getMemberLookup().ContainsKey(name))
                {
                    return walker.getMemberLookup()[name];
                }
                walker = walker.nestParent;
            }

            foreach (ImportStatement impStmnt in this.activeEntity.fileContext.imports)
            {
                if (impStmnt.isPollutionImport)
                {
                    if (impStmnt.compiledModuleRef.nestedEntities.ContainsKey(name))
                    {
                        return impStmnt.compiledModuleRef.nestedEntities[name];
                    }
                }
            }

            return null;
        }

        // Note that you may get an enum when passing in the fully qualified member name.
        internal AbstractEntity DoLookup(string ns, string dottedName)
        {
            string name = dottedName.Split('.')[0];

            if (ns == "") return this.DoDirectLookup(name);

            AbstractEntity e = this.DoDirectLookup(ns + "." + name);
            if (e != null) return e;

            string[] parts = ns.Split('.');
            for (int i = 1; i < parts.Length; i++)
            {
                parts[i] = parts[i - 1] + "." + parts[i];
            }

            for (int i = parts.Length - 1; i >= 0; i--)
            {
                e = this.DoDirectLookup(parts[i] + "." + name);
                if (e != null) return e;
            }

            return null;
        }

        internal string GetCurrentNamespace()
        {
            if (this.activeEntity.nestParent == null) return "";
            return this.activeEntity.nestParent.fqName;
        }
    }
}
