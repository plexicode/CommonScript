using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class CompiledModule
    {
        public string id;
        public System.Collections.Generic.Dictionary<string, string> codeFiles;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> nestedEntities;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntities;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> entitiesNoEnumParents;
        public System.Collections.Generic.List<AbstractEntity> lambdaEntities;
        public System.Collections.Generic.Dictionary<string, ModuleResource> embeddedResources;

        public CompiledModule(string id, System.Collections.Generic.Dictionary<string, string> codeFiles, System.Collections.Generic.Dictionary<string, AbstractEntity> nestedEntities, System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntities, System.Collections.Generic.Dictionary<string, AbstractEntity> entitiesNoEnumParents, System.Collections.Generic.List<AbstractEntity> lambdaEntities, System.Collections.Generic.Dictionary<string, ModuleResource> embeddedResources)
        {
            this.id = id;
            this.codeFiles = codeFiles;
            this.nestedEntities = nestedEntities;
            this.flattenedEntities = flattenedEntities;
            this.entitiesNoEnumParents = entitiesNoEnumParents;
            this.lambdaEntities = lambdaEntities;
            this.embeddedResources = embeddedResources;
        }
    }
}
