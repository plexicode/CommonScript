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

            FunctionWrapper.PerformFullResolutionPassOnConstAndEnums(resolver, constAndEnumResolutionOrder);

            ClassEntity[] orderedClasses = FunctionWrapper.ResolveBaseClassesAndEstablishClassOrder(resolver, classes, resolver.flattenedEntities);

            for (i = 0; i < orderedClasses.Length; i += 1)
            {
                FunctionWrapper.EntityResolver_DetermineMemberOffsets(orderedClasses[i]);
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

    }
}
