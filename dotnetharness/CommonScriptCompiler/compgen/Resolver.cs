using System.Collections.Generic;

namespace CommonScript.Compiler.Internal
{
    public class Resolver
    {
        public StaticContext staticCtx;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> nestedEntities;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> enumsByMemberFqName;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntities;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntitiesAndEnumValues;
        public System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntitiesNoEnumParents;
        public System.Collections.Generic.List<FunctionEntity> lambdas;
        public AbstractEntity activeEntity;
        public AbstractEntity[] entityList;
        public Statement breakContext;
        public int autoVarId;
        public StringSet extensionNames;

        public Resolver(StaticContext staticCtx, System.Collections.Generic.Dictionary<string, AbstractEntity> nestedEntities, System.Collections.Generic.Dictionary<string, AbstractEntity> enumsByMemberFqName, System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntities, System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntitiesAndEnumValues, System.Collections.Generic.Dictionary<string, AbstractEntity> flattenedEntitiesNoEnumParents, System.Collections.Generic.List<FunctionEntity> lambdas, AbstractEntity activeEntity, AbstractEntity[] entityList, Statement breakContext, int autoVarId, StringSet extensionNames)
        {
            this.staticCtx = staticCtx;
            this.nestedEntities = nestedEntities;
            this.enumsByMemberFqName = enumsByMemberFqName;
            this.flattenedEntities = flattenedEntities;
            this.flattenedEntitiesAndEnumValues = flattenedEntitiesAndEnumValues;
            this.flattenedEntitiesNoEnumParents = flattenedEntitiesNoEnumParents;
            this.lambdas = lambdas;
            this.activeEntity = activeEntity;
            this.entityList = entityList;
            this.breakContext = breakContext;
            this.autoVarId = autoVarId;
            this.extensionNames = extensionNames;
        }
    }
}
