﻿using System;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class LookupEngine
    {
        // Determines if a variable is actually a pointer into any of the imported modules. 
        public static Expression DoFirstPassVariableLookupThroughImports(Resolver ctx, Token refToken, string name)
        {
            FileContext fileCtx = (FileContext)ctx.activeEntity.OBJ_TEMP_CAST_fileContext;

            // If there's an import into a name, this is considered a locally defined name,
            // and therefore will take precedence over pollution imports.
            if (fileCtx.importsByVar.ContainsKey(name))
            {
                return Expression.createImportReference(refToken, fileCtx.importsByVar[name]);
            }

            ImportStatement[] imports = fileCtx.imports;
            for (int i = imports.Length - 1; i >= 0; i--)
            {
                ImportStatement importStatement = imports[i];
                if (importStatement.isPollutionImport)
                {
                    CompiledModule mod = importStatement.compiledModuleRef;
                    Expression referenceExpression = tryCreateModuleMemberReference(mod, refToken, name);
                    if (referenceExpression != null)
                    {
                        return referenceExpression;
                    }
                }
            }

            return null;
        }

        public static Expression tryCreateModuleMemberReference(CompiledModule mod, Token refToken, string name)
        {
            if (mod.nestedEntities.ContainsKey(name))
            {
                AbstractEntity tle = mod.nestedEntities[name];
                switch (tle.type)
                {
                    case (int)EntityType.FUNCTION:
                        return Expression.createFunctionReference(refToken, name, tle);
                    case (int)EntityType.CONST:
                        return ((ConstEntity)tle.specificData).constValue;
                    case (int)EntityType.CLASS:
                        return Expression.createClassReference(refToken, tle);
                    case (int)EntityType.NAMESPACE:
                        return Expression.createNamespaceReference(refToken, tle);
                    default:
                        throw new NotImplementedException();
                }
            }
            return null;
        }
    }
}
