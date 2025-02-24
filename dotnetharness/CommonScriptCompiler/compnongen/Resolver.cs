using System;
using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ResolverUtil
    {
        public static void Resolve(Resolver resolver)
        {
            int i = 0;
            
            List<FunctionEntity> functions = new List<FunctionEntity>();
            List<ClassEntity> classes = new List<ClassEntity>();
            List<ConstEntity> constants = new List<ConstEntity>();
            List<EnumEntity> enums = new List<EnumEntity>();
            List<FunctionEntity> constructors = new List<FunctionEntity>();
            List<FieldEntity> fields = new List<FieldEntity>();

            AbstractEntity[] entities = resolver.flattenedEntities.Values.ToArray();

            for (i = 0; i < entities.Length; i += 1)
            {
                AbstractEntity tle = entities[i];

                if (tle.type == (int)EntityType.CONST)
                {
                    constants.Add((ConstEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.ENUM)
                {
                    enums.Add((EnumEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.FUNCTION)
                {
                    functions.Add((FunctionEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CLASS)
                {
                    classes.Add((ClassEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.CONSTRUCTOR)
                {
                    constructors.Add((FunctionEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.FIELD)
                {
                    fields.Add((FieldEntity)tle.specificData);
                }
                else if (tle.type == (int)EntityType.NAMESPACE)
                {
                    // Nothing to resolve.
                }
                else
                {
                    FunctionWrapper.fail("Not implemented");
                }
            }

            FunctionWrapper.AddImplicitIncrementingEnumValueDefinitions(enums);

            string[] constAndEnumResolutionOrder = FunctionWrapper.Resolver_DetermineConstAndEnumResolutionOrder(resolver, constants, enums);

            PerformFullResolutionPassOnConstAndEnums(resolver, constAndEnumResolutionOrder);

            ClassEntity[] orderedClasses = ResolveBaseClassesAndEstablishClassOrder(resolver, classes, resolver.flattenedEntities);

            for (i = 0; i < orderedClasses.Length; i += 1)
            {
                EntityResolverUtil.EntityResolver_DetermineMemberOffsets(orderedClasses[i]);
            }

            for (i = 0; i < functions.Count; i += 1)
            {
                FunctionWrapper.EntityResolver_ResetAutoVarId(resolver);
                FunctionWrapper.EntityResolver_ResolveFunctionFirstPass(resolver, functions[i]);
            }

            for (i = 0; i < constructors.Count; i += 1)
            {
                FunctionWrapper.EntityResolver_ResetAutoVarId(resolver);
                FunctionWrapper.EntityResolver_ResolveFunctionFirstPass(resolver, constructors[i]);
            }

            // At this point, all lambdas have been reported.
            // DO NOT CHANGE TO FOR-EACH. This count can grow as a result of nested lambdas.
            for (i = 0; i < resolver.lambdas.Count; i += 1) 
            {
                FunctionWrapper.EntityResolver_ResetAutoVarId(resolver);
                FunctionWrapper.EntityResolver_ResolveFunctionFirstPass(resolver, resolver.lambdas[i]);
            }

            for (i = 0; i < functions.Count; i += 1)
            {
                FunctionWrapper.EntityResolver_ResolveFunctionSecondPass(resolver, functions[i]);
            }

            for (i = 0; i < constructors.Count; i += 1)
            {
                FunctionWrapper.EntityResolver_ResolveFunctionSecondPass(resolver, constructors[i]);
            }

            for (i = 0; i < resolver.lambdas.Count; i += 1)
            {
                FunctionWrapper.EntityResolver_ResolveFunctionSecondPass(resolver, resolver.lambdas[i]);
            }
        }

        private static ClassEntity[] ResolveBaseClassesAndEstablishClassOrder(
            Resolver resolver, 
            List<ClassEntity> classes,
            Dictionary<string, AbstractEntity> flattenedEntities)
        {
            int i = 0;
            int j = 0;
            
            // TODO: add external module resolving as a separate feature.

            ClassEntity[] deterministicOrder = FunctionWrapper.ClassSorter_SortClassesInDeterministicDependencyOrder(
                classes.ToArray(), false);
            
            List<ClassEntity> finalOrder = new List<ClassEntity>();
            List<ClassEntity> baseClassRequired = new List<ClassEntity>();
            for (i = 0; i < deterministicOrder.Length; i += 1)
            {
                ClassEntity e = deterministicOrder[i];
                if (e.baseClassTokens != null)
                {
                    baseClassRequired.Add(e);
                }
                else
                {
                    finalOrder.Add(e);
                }
            }
            
            for (i = 0; i < baseClassRequired.Count; i += 1)
            {
                ClassEntity bc = baseClassRequired[i];
                resolver.activeEntity = bc.baseData;
                Token baseClassToken = bc.baseClassTokens[0];
                AbstractEntity bcEntity = FunctionWrapper.LookupUtil_DoLookupForName(resolver, baseClassToken, baseClassToken.Value);
                if (bcEntity != null)
                {
                    for (j = 2; j < bc.baseClassTokens.Length; j += 2)
                    {
                        string next = bc.baseClassTokens[j].Value;
                        if (bcEntity != null)
                        {
                            Dictionary<string, AbstractEntity> lookup = FunctionWrapper.Entity_getMemberLookup(resolver.staticCtx, bcEntity);
                            if (lookup.ContainsKey(next))
                            {
                                bcEntity = lookup[next];
                            }
                            else
                            {
                                bcEntity = null;
                            }
                        }
                    }
                }
                if (bcEntity == null)
                {
                    FunctionWrapper.Errors_Throw(bc.baseData.firstToken, "Could not resolve base class");
                    FunctionWrapper.fail("Not implemented");
                }
                if (bcEntity.type != (int)EntityType.CLASS)
                {
                    FunctionWrapper.Errors_Throw(bc.baseData.firstToken, bcEntity.fqName + " is not a valid class.");
                }

                bc.baseClassEntity = (ClassEntity)bcEntity.specificData;
                resolver.activeEntity = null;
            }

            Dictionary<string, bool> includedInOrder = new Dictionary<string, bool>();
            for (i = 0; i < baseClassRequired.Count; i += 1)
            {
                ClassEntity bc = baseClassRequired[i];
                includedInOrder[bc.baseData.fqName] = false;
            }
            
            for (i = 0; i < baseClassRequired.Count; i += 1)
            {
                ClassEntity bc = baseClassRequired[i];
                ClassEntity walker = bc;
                List<ClassEntity> order = new List<ClassEntity>();
                while (walker != null)
                {
                    // If it's external or already included, stop.
                    if (!includedInOrder.ContainsKey(walker.baseData.fqName) ||
                        includedInOrder[walker.baseData.fqName])
                    {
                        walker = null;
                    }
                    else
                    {
                        order.Add(walker);
                        includedInOrder[walker.baseData.fqName] = true;
                        walker = walker.baseClassEntity;
                    }

                    if (order.Count > deterministicOrder.Length)
                    {
                        FunctionWrapper.Errors_Throw(bc.baseData.firstToken, "This class has a cycle in its base class chain.");
                    }
                }

                order.Reverse();
                for (j = 0; j < order.Count; j += 1)
                {
                    finalOrder.Add(order[j]);
                }
            }

            return finalOrder.ToArray();
        }

        private static void PerformFullResolutionPassOnConstAndEnums(Resolver resolver, string[] resOrder)
        {
            for (int passNum = 1; passNum <= 2; passNum += 1)
            {
                for (int i = 0; i < resOrder.Length; i += 1)
                {
                    AbstractEntity entity = resolver.flattenedEntitiesAndEnumValues[resOrder[i]];
                    resolver.activeEntity = entity;
                    if (entity.type == (int)EntityType.ENUM)
                    {
                        EnumEntity enumDef = (EnumEntity)entity.specificData;
                        int memberIndex = FunctionWrapper.Resolver_GetEnumMemberIndex(resolver, resOrder[i], enumDef);
                        Expression val = enumDef.memberValues[memberIndex];
                        if (passNum == 1)
                        {
                            val = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
                        }
                        else
                        {
                            val = FunctionWrapper.ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
                        }

                        if (passNum == 2 && val.type != (int) ExpressionType.INTEGER_CONST)
                        {
                            FunctionWrapper.Errors_Throw(enumDef.memberNameTokens[memberIndex], "This enum value has a non-integer value.");
                        }

                        enumDef.memberValues[memberIndex] = val;
                    }
                    else
                    {
                        ConstEntity constEnt = (ConstEntity)entity.specificData;
                        Expression val = constEnt.constValue;
                        if (passNum == 1)
                        {
                            val = FunctionWrapper.ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
                        }
                        else
                        {
                            val = FunctionWrapper.ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
                            if (!FunctionWrapper.IsExpressionConstant(val))
                            {
                                FunctionWrapper.Errors_Throw(val.firstToken, "A constant expression is required here.");
                            }
                        }

                        constEnt.constValue = val;
                    }
                    resolver.activeEntity = null;
                }
            }
        }
    }
}
