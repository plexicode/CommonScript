using System.Collections.Generic;
using System.Linq;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class CompiledModuleUtil
    {
        public static void AddLambdas(CompiledModule m, IList<FunctionEntity> lambdas)
        {
            m.lambdaEntities = [.. lambdas.Select(fn => fn.baseData)];
        }

        public static void InitializeCompieldModuleLookups(
            CompiledModule m,
            Dictionary<string, AbstractEntity> rootEntities,
            Dictionary<string, AbstractEntity> flatEntities)
        {
            m.nestedEntities = rootEntities;
            m.flattenedEntities = flatEntities;
            m.entitiesNoEnumParents = new Dictionary<string, AbstractEntity>();
            foreach (string fqName in m.flattenedEntities.Keys)
            {
                AbstractEntity entity = m.flattenedEntities[fqName];
                if (entity.type == (int)EntityType.ENUM)
                {
                    // for no-enum-parents, add all the children but not the entity itself. This is
                    // used for situations where a specific enum member is desired instead of the definition.
                    foreach (Token enumMem in ((EnumEntity)entity.specificData).memberNameTokens)
                    {
                        m.entitiesNoEnumParents[fqName + "." + enumMem.Value] = entity;
                    }
                }
                else
                {
                    m.entitiesNoEnumParents[fqName] = entity;
                }
            }
        }
    }
}
