using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal class CompiledModule
    {
        public string id { get; set; }
        public Dictionary<string, string> codeFiles;
        public Dictionary<string, string> textResources;
        public Dictionary<string, AbstractEntity> nestedEntities;
        public Dictionary<string, AbstractEntity> flattenedEntities;
        public Dictionary<string, AbstractEntity> entitiesNoEnumParents;
        public List<FunctionLikeEntity> lambdaEntities;

        public CompiledModule(string id)
        {
            this.id = id;
            this.codeFiles = new Dictionary<string, string>();
            this.textResources = new Dictionary<string, string>();
        }

        public void AddLambdas(IList<FunctionLikeEntity> lambdas)
        {
            this.lambdaEntities = [.. lambdas];
        }

        public void InitializeCompieldModuleLookups(
            Dictionary<string, AbstractEntity> rootEntities,
            Dictionary<string, AbstractEntity> flatEntities)
        {
            this.nestedEntities = rootEntities;
            this.flattenedEntities = flatEntities;
            this.entitiesNoEnumParents = new Dictionary<string, AbstractEntity>();
            foreach (string fqName in this.flattenedEntities.Keys)
            {
                AbstractEntity entity = this.flattenedEntities[fqName];
                if (entity.type == EntityType.ENUM)
                {
                    // for no-enum-parents, add all the children but not the entity itself. This is
                    // used for situations where a specific enum member is desired instead of the definition.
                    foreach (Token enumMem in ((EnumEntity)entity.specificData).memberNameTokens)
                    {
                        this.entitiesNoEnumParents[fqName + "." + enumMem.Value] = entity;
                    }
                }
                else
                {
                    this.entitiesNoEnumParents[fqName] = entity;
                }
            }
        }
    }

    internal class CompiledFunctionMetadata
    {
        public string fqName { get; set; }
        public int argc { get; set; }
        public int argcMin { get; set; }
        public int pc { get; set; }
    }

    internal class CompiledByteCodeRow
    {
        public int op { get; set; }
        public int[] args { get; set; }
        public string stringArg { get; set; }
        public string stringId { get; set; }
        public Token token { get; set; }
    }
}
