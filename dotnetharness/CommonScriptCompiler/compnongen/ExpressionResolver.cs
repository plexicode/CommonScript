﻿using System;
using System.Collections.Generic;

namespace CommonScript.Compiler
{
    internal class ExpressionResolver
    {
        private Resolver resolver;
        public StatementResolver statementResolver;
        public EntityResolver entityResolver;

        public ExpressionResolver(Resolver resolver)
        {
            this.resolver = resolver;
        }

        public Expression ResolveExpressionFirstPass(Expression expr)
        {
            switch (expr.type)
            {
                case ExpressionType.BASE_CTOR_REFERENCE: return this.FirstPass_BaseCtorReference(expr);
                case ExpressionType.BINARY_OP: return this.FirstPass_BinaryOp(expr);
                case ExpressionType.BITWISE_NOT: return this.FirstPass_BitwiseNot(expr);
                case ExpressionType.BOOL_CONST: return this.FirstPass_BoolConst(expr);
                case ExpressionType.BOOLEAN_NOT: return this.FirstPass_BoolNot(expr);
                case ExpressionType.CONSTRUCTOR_INVOCATION: return this.FirstPass_ConstructorInvocation(expr);
                case ExpressionType.CONSTRUCTOR_REFERENCE: return this.FirstPass_ConstructorReference(expr);
                case ExpressionType.DICTIONARY_DEFINITION: return this.FirstPass_DictionaryDefinition(expr);
                case ExpressionType.DOT_FIELD: return this.FirstPass_DotField(expr);
                case ExpressionType.FLOAT_CONST: return this.FirstPass_FloatConstant(expr);
                case ExpressionType.FUNCTION_INVOKE: return this.FirstPass_FunctionInvocation(expr);
                case ExpressionType.INDEX: return this.FirstPass_Index(expr);
                case ExpressionType.INLINE_INCREMENT: return this.FirstPass_InlineIncrement(expr);
                case ExpressionType.INTEGER_CONST: return this.FirstPass_IntegerConstant(expr);
                case ExpressionType.LAMBDA: return this.FirstPass_Lambda(expr);
                case ExpressionType.LIST_DEFINITION: return this.FirstPass_ListDefinition(expr);
                case ExpressionType.NEGATIVE_SIGN: return this.FirstPass_NegativeSign(expr);
                case ExpressionType.NULL_CONST: return this.FirstPass_NullConst(expr);
                case ExpressionType.SLICE: return this.FirstPass_Slice(expr);
                case ExpressionType.STRING_CONST: return this.FirstPass_StringConstant(expr);
                case ExpressionType.TERNARY: return this.FirstPass_Ternary(expr);
                case ExpressionType.THIS: return this.FirstPass_This(expr);
                case ExpressionType.TYPEOF: return this.FirstPass_TypeOf(expr);
                case ExpressionType.VARIABLE: return this.FirstPass_Variable(expr);

                case ExpressionType.EXTENSION_REFERENCE:
                    Errors.ThrowError(expr.firstToken, "Extension method references must be invoked immediately.");
                    break;

                default:
                    Errors.ThrowNotImplemented(expr.firstToken, "first pass for this type");
                    break;
            }
            return expr;
        }

        public Expression ResolveExpressionSecondPass(Expression expr)
        {
            switch (expr.type)
            {
                case ExpressionType.BASE_CTOR_REFERENCE: return this.SecondPass_BaseCtorReference(expr);
                case ExpressionType.BINARY_OP: return this.SecondPass_BinaryOp(expr);
                case ExpressionType.BITWISE_NOT: return this.SecondPass_BitwiseNot(expr);
                case ExpressionType.BOOL_CONST: return this.SecondPass_BoolConst(expr);
                case ExpressionType.BOOLEAN_NOT: return this.SecondPass_BoolNot(expr);
                case ExpressionType.CLASS_REFERENCE: return this.SecondPass_ClassReference(expr);
                case ExpressionType.CONSTRUCTOR_REFERENCE: return this.SecondPass_ConstructorReference(expr, false);
                case ExpressionType.DICTIONARY_DEFINITION: return this.SecondPass_DictionaryDefinition(expr);
                case ExpressionType.DOT_FIELD: return this.SecondPass_DotField(expr);
                case ExpressionType.ENUM_CONST: return this.SecondPass_EnumConstant(expr);
                case ExpressionType.EXTENSION_INVOCATION: return this.SecondPass_ExtensionInvocation(expr);
                case ExpressionType.FLOAT_CONST: return this.SecondPass_FloatConstant(expr);
                case ExpressionType.FUNCTION_INVOKE: return this.SecondPass_FunctionInvocation(expr);
                case ExpressionType.FUNCTION_REFERENCE: return this.SecondPass_FunctionReference(expr);
                case ExpressionType.IMPORT_REFERENCE: return this.SecondPass_ImportReference(expr);
                case ExpressionType.INDEX: return this.SecondPass_Index(expr);
                case ExpressionType.INLINE_INCREMENT: return this.SecondPass_InlineIncrement(expr);
                case ExpressionType.INTEGER_CONST: return this.SecondPass_IntegerConstant(expr);
                case ExpressionType.LAMBDA: return this.SecondPass_Lambda(expr);
                case ExpressionType.LIST_DEFINITION: return this.SecondPass_ListDefinition(expr);
                case ExpressionType.NAMESPACE_REFERENCE: return this.SecondPass_NamespaceReference(expr);
                case ExpressionType.NEGATIVE_SIGN: return this.SecondPass_NegativeSign(expr);
                case ExpressionType.NULL_CONST: return this.SecondPass_NullConstant(expr);
                case ExpressionType.SLICE: return this.SecondPass_Slice(expr);
                case ExpressionType.STRING_CONST: return this.SecondPass_StringConstant(expr);
                case ExpressionType.TERNARY: return this.SecondPass_Ternary(expr);
                case ExpressionType.THIS: return this.SecondPass_ThisConstant(expr);
                case ExpressionType.TYPEOF: return this.SecondPass_TypeOf(expr);
                case ExpressionType.VARIABLE: return this.SecondPass_Variable(expr);
                default:
                    Errors.ThrowNotImplemented(expr.firstToken, "second pass for this type");
                    break;
            }
            return expr;
        }

        public void ResolveExpressionArrayFirstPass(Expression[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.ResolveExpressionFirstPass(arr[i]);
            }
        }

        public void ResolveExpressionArraySecondPass(Expression[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = this.ResolveExpressionSecondPass(arr[i]);
            }
        }

        private Expression FirstPass_BaseCtorReference(Expression baseCtor)
        {
            return baseCtor;
        }

        private Expression FirstPass_BinaryOp(Expression binOp)
        {
            binOp.left = this.ResolveExpressionFirstPass(binOp.left);
            binOp.right = this.ResolveExpressionFirstPass(binOp.right);
            switch (binOp.opToken.Value)
            {
                case "|":
                case "&":
                case "^":
                case "<<":
                case ">>":
                    binOp.left = this.IntegerRequired(binOp.left);
                    binOp.right = this.IntegerRequired(binOp.right);
                    break;
            }
            return binOp;
        }

        private Expression FirstPass_BitwiseNot(Expression bwn)
        {
            bwn.root = this.ResolveExpressionFirstPass(bwn.root);
            bwn.root = this.IntegerRequired(bwn.root);
            return bwn;
        }

        private Expression FirstPass_BoolConst(Expression bc)
        {
            return bc;
        }

        private Expression FirstPass_BoolNot(Expression booNot)
        {
            booNot.root = this.ResolveExpressionFirstPass(booNot.root);
            return booNot;
        }

        private Expression FirstPass_ConstructorInvocation(Expression ctorInvoke)
        {
            // You left off here realizing that it's actually better to just use CONSTRUCTOR_REF as a parse node, that way 
            // the resolver for the path of the item can be resolve in a normal way e.g. "new myModuleAlias.SomeClass()" should
            // resolve as an invocation of a class reference expression. 
            throw new NotImplementedException();
        }

        private Expression FirstPass_ConstructorReference(Expression ctorRef)
        {
            ctorRef.root = this.ResolveExpressionFirstPass(ctorRef.root);
            return ctorRef;
        }

        private Expression FirstPass_DictionaryDefinition(Expression dictDef)
        {
            int length = dictDef.keys.Length;
            for (int i = 0; i < length; i++)
            {
                dictDef.keys[i] = this.ResolveExpressionFirstPass(dictDef.keys[i]);
                dictDef.values[i] = this.ResolveExpressionFirstPass(dictDef.values[i]);
            }
            return dictDef;
        }

        private Expression FirstPass_DotField(Expression dotField)
        {
            dotField.root = this.ResolveExpressionFirstPass(dotField.root);
            string fieldName = dotField.strVal;

            switch (dotField.root.type)
            {
                case ExpressionType.IMPORT_REFERENCE:
                    ImportStatement importRef = (ImportStatement)dotField.root.objPtr;
                    CompiledModule moduleRef = importRef.compiledModuleRef;
                    Expression output = LookupEngine.tryCreateModuleMemberReference(moduleRef, dotField.firstToken, fieldName);
                    if (output == null)
                    {
                        Errors.ThrowError(dotField.opToken, "The module does not have a member named '" + fieldName + "'");
                    }
                    return output;

                case ExpressionType.ENUM_REFERENCE:
                    EnumEntity enumRef = (EnumEntity)dotField.root.objPtr;
                    // TODO: ew, gross, figure out a way to cache and stash a dictionary somewhere on the entity.
                    // Otherwise this could potentitially turn problematic in the off chance there's an enum with a
                    // few hundred members in it (e.g. generated code).
                    for (int i = 0; i < enumRef.memberNameTokens.Length; i++)
                    {
                        if (enumRef.memberNameTokens[i].Value == fieldName)
                        {
                            return Expression.createEnumConstant(dotField.firstToken, enumRef, fieldName, enumRef.memberValues[i].intVal);
                        }
                    }
                    Errors.ThrowError(dotField.opToken, "The enum " + enumRef.fqName + " does not have a member named '" + fieldName + "'");
                    break;

                case ExpressionType.NAMESPACE_REFERENCE:
                    NamespaceEntity nsEntity = (NamespaceEntity)dotField.root.objPtr;
                    if (!nsEntity.nestedMembers.ContainsKey(fieldName))
                    {
                        Errors.ThrowError(dotField.opToken, "There is no member of this namespace named '" + fieldName + "'.");
                    }
                    AbstractEntity referencedEntity = nsEntity.nestedMembers[fieldName];
                    return WrapEntityIntoReferenceExpression(dotField.firstToken, referencedEntity);
            }

            return dotField;
        }

        private Expression FirstPass_FloatConstant(Expression floatConst)
        {
            return floatConst;
        }

        private Expression FirstPass_FunctionInvocation(Expression funcInvoke)
        {
            if (funcInvoke.root.type == ExpressionType.EXTENSION_REFERENCE)
            {
                this.ResolveExpressionArrayFirstPass(funcInvoke.args);
                return Expression.createExtensionInvocation(funcInvoke.firstToken, funcInvoke.root.strVal, funcInvoke.args);
            }
            else
            {
                funcInvoke.root = this.ResolveExpressionFirstPass(funcInvoke.root);
                this.ResolveExpressionArrayFirstPass(funcInvoke.args);
            }

            return funcInvoke;
        }

        private Expression FirstPass_Index(Expression indexExpr)
        {
            indexExpr.root = this.ResolveExpressionFirstPass(indexExpr.root);
            indexExpr.right = this.ResolveExpressionFirstPass(indexExpr.right);
            return indexExpr;
        }

        private Expression FirstPass_InlineIncrement(Expression inlineIncr)
        {
            inlineIncr.root = this.ResolveExpressionFirstPass(inlineIncr.root);
            return inlineIncr;
        }

        private Expression FirstPass_IntegerConstant(Expression intConst)
        {
            return intConst;
        }

        private Expression FirstPass_Lambda(Expression lamb)
        {
            FunctionLikeEntity lambdaEnt = FunctionLikeEntity.BuildLambda(
                this.resolver.activeEntity.fileContext,
                lamb.firstToken,
                lamb.argNames,
                lamb.values,
                lamb.nestedCode);
            this.resolver.ReportNewLambda(lambdaEnt);
            lamb.objPtr = lambdaEnt;
            return lamb;
        }

        private Expression FirstPass_ListDefinition(Expression listDef)
        {
            for (int i = 0; i < listDef.values.Length; i++)
            {
                listDef.values[i] = this.ResolveExpressionFirstPass(listDef.values[i]);
            }
            return listDef;
        }

        private Expression FirstPass_NegativeSign(Expression negSign)
        {
            negSign.root = this.ResolveExpressionFirstPass(negSign.root);
            return negSign;
        }

        private Expression FirstPass_NullConst(Expression nullConst)
        {
            return nullConst;
        }

        private Expression FirstPass_Slice(Expression slice)
        {
            slice.root = this.ResolveExpressionFirstPass(slice.root);
            for (int i = 0; i < 3; i++)
            {
                if (slice.args[i] != null) slice.args[i] = this.ResolveExpressionFirstPass(slice.args[i]);
            }
            return slice;
        }

        private Expression FirstPass_StringConstant(Expression strConst)
        {
            return strConst;
        }

        private Expression FirstPass_Ternary(Expression ternary)
        {
            ternary.root = this.ResolveExpressionFirstPass(ternary.root);
            ternary.left = this.ResolveExpressionFirstPass(ternary.left);
            ternary.right = this.ResolveExpressionFirstPass(ternary.right);
            return ternary;
        }

        private Expression FirstPass_This(Expression thisExpr)
        {
            return thisExpr;
        }

        private Expression FirstPass_TypeOf(Expression typeofExpr)
        {
            typeofExpr.root = this.ResolveExpressionFirstPass(typeofExpr.root);
            return typeofExpr;
        }

        private static AbstractEntity FindLocallyReferencedEntity(Dictionary<string, AbstractEntity> lookup, string name)
        {
            if (lookup.ContainsKey(name))
            {
                return lookup[name];
            }

            if (lookup.ContainsKey(".."))
            {
                return FindLocallyReferencedEntity(lookup[".."].getMemberLookup(), name);
            }
            return null;
        }

        private Expression WrapEntityIntoReferenceExpression(Token token, AbstractEntity entity)
        {
            switch (entity.type)
            {
                case EntityType.FUNCTION:
                    return Expression.createFunctionReference(token, entity.simpleName, entity);

                case EntityType.CLASS:
                    return Expression.createClassReference(token, entity);

                case EntityType.CONST:
                    return Expression.cloneExpressionWithNewToken(token, ((ConstEntity)entity).constValue);

                case EntityType.ENUM:
                    return Expression.createEnumReference(token, entity);

                case EntityType.NAMESPACE:
                    return Expression.createNamespaceReference(token, entity);
            }

            Errors.ThrowError(token, "Not implemented!");
            return null;
        }

        private Expression FirstPass_Variable(Expression varExpr)
        {
            string name = varExpr.strVal;

            AbstractEntity localEntity = FindLocallyReferencedEntity(this.resolver.nestedEntities, name);
            if (localEntity != null)
            {
                return WrapEntityIntoReferenceExpression(varExpr.firstToken, localEntity);
            }

            Expression importedRef = LookupEngine.DoFirstPassVariableLookupThroughImports(this.resolver, varExpr.firstToken, name);
            if (importedRef != null) return importedRef;

            return varExpr;
        }

        private static bool IsExpressionNumericConstant(Expression expr)
        {
            ExpressionType t = expr.type;
            return
                t == ExpressionType.INTEGER_CONST ||
                t == ExpressionType.FLOAT_CONST ||
                t == ExpressionType.ENUM_CONST;
        }

        private static double GetNumericValueOfConstant(Expression exprConst)
        {
            return exprConst.type == ExpressionType.FLOAT_CONST ? exprConst.floatVal : exprConst.intVal;
        }


        private static string SimpleExprToTypeName(ExpressionType t)
        {
            switch (t)
            {
                case ExpressionType.BOOL_CONST: return "boolean";
                case ExpressionType.INTEGER_CONST: return "integer";
                case ExpressionType.FLOAT_CONST: return "float";
                case ExpressionType.NULL_CONST: return "null";
                case ExpressionType.STRING_CONST: return "string";
                case ExpressionType.ENUM_CONST: return "enum";
            }
            throw new NotImplementedException(); // this should not happen
        }
        private static void ThrowOpNotDefinedError(Token throwToken, string op, ExpressionType left, ExpressionType right)
        {
            Errors.ThrowError(throwToken, "The operation '" + SimpleExprToTypeName(left) + " " + op + " " + SimpleExprToTypeName(right) + "' is not defined.");
        }

        private Expression SecondPass_BaseCtorReference(Expression baseCtor)
        {
            baseCtor.objPtr = ((ClassEntity)this.resolver.activeEntity.nestParent).baseClassEntity;
            return baseCtor;
        }

        private Expression SecondPass_BinaryOp(Expression expr)
        {
            Token firstToken = expr.firstToken;
            Token opToken = expr.opToken;
            string op = opToken.Value;

            expr.left = this.ResolveExpressionSecondPass(expr.left);
            if (expr.right.type == ExpressionType.CLASS_REFERENCE && op == "is")
            {
                expr.right.boolVal = true;
            }
            expr.right = this.ResolveExpressionSecondPass(expr.right);


            // TODO: resolve constants
            if (Resolver.IsExpressionConstant(expr.left) && Resolver.IsExpressionConstant(expr.right))
            {
                bool isLeftNumeric = IsExpressionNumericConstant(expr.left);
                bool isRightNumeric = IsExpressionNumericConstant(expr.right);
                bool isRightZero = expr.right.type == ExpressionType.FLOAT_CONST ? (expr.right.floatVal == 0) : (expr.right.intVal == 0);

                if (isRightNumeric)
                {
                    if (op == "/" || op == "%")
                    {
                        if (isRightZero)
                        {
                            if (op == "%") Errors.ThrowError(opToken, "Modulo by zero");
                            Errors.ThrowError(opToken, "Division by zero");
                        }
                    }
                }

                if (isLeftNumeric && isRightNumeric)
                {
                    double floatLeft = GetNumericValueOfConstant(expr.left);
                    double floatRight = GetNumericValueOfConstant(expr.right);

                    if (op == "<") return Expression.createBoolConstant(firstToken, floatLeft < floatRight);
                    if (op == ">") return Expression.createBoolConstant(firstToken, floatLeft > floatRight);
                    if (op == "<=") return Expression.createBoolConstant(firstToken, floatLeft <= floatRight);
                    if (op == ">=") return Expression.createBoolConstant(firstToken, floatLeft >= floatRight);

                    // TODO
                    // strict equality comparison on floats is okay in the case that these are converted
                    // from integers because they'll still be whole numbers. There is a bug here where they'll
                    // be inaccurate for float comparisons which is to be expected BUT they'll be slightly
                    // inaccurate in different ways across platforms.
                    // As a hypothetical example:
                    //     value = ln(e ** 2) == 2;
                    //       vs
                    //     x = 2
                    //     value = ln(e ** x) == x;
                    // This value may different if the runtime language and compile time language are different.
                    if (op == "==") return Expression.createBoolConstant(firstToken, floatLeft == floatRight);
                    if (op == "!=") return Expression.createBoolConstant(firstToken, floatLeft != floatRight);
                }

                if (op == "+" && (expr.left.type == ExpressionType.STRING_CONST || expr.right.type == ExpressionType.STRING_CONST))
                {
                    string leftStr = GetStringFromConstantExpression(expr.left);
                    string rightStr = GetStringFromConstantExpression(expr.right);
                    return Expression.createStringConstant(expr.firstToken, leftStr + rightStr);
                }

                switch ((int)expr.left.type * (int)ExpressionType.MAX_VALUE + (int)expr.right.type)
                {
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                        int intLeft = expr.left.intVal;
                        int intRight = expr.right.intVal;
                        if (op == "**")
                        {
                            return Expression.createFloatConstant(opToken, Math.Pow(intLeft, intRight));
                        }
                        int resultInt = 0;
                        switch (op)
                        {
                            case "+": resultInt = intLeft + intRight; break;
                            case "-": resultInt = intLeft - intRight; break;
                            case "*": resultInt = intLeft * intRight; break;
                            case "/": resultInt = intLeft / intRight; break;
                            case "%":
                                if (intRight < 0)
                                {
                                    intRight = -intRight;
                                    resultInt = intLeft % intRight;
                                    if (resultInt > 0) resultInt -= intRight;
                                }
                                else
                                {
                                    resultInt = intLeft % intRight;
                                    if (resultInt < 0) resultInt += intRight;
                                }
                                break;
                            case "|": resultInt = intLeft | intRight; break;
                            case "&": resultInt = intLeft & intRight; break;
                            case "^": resultInt = intLeft ^ intRight; break;
                            case "<<": resultInt = intLeft << intRight; break;
                            case ">>": resultInt = intLeft >> intRight; break;
                            case ">>>": resultInt = intLeft >>> intRight; break;

                            default:
                                ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                                break;
                        }
                        return Expression.createIntegerConstant(opToken, resultInt);

                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.ENUM_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.FLOAT_CONST:
                    case (int)ExpressionType.FLOAT_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.ENUM_CONST:
                        double floatLeft = GetNumericValueOfConstant(expr.left);
                        double floatRight = GetNumericValueOfConstant(expr.right);
                        double floatResult = 0.0;
                        switch (op)
                        {
                            case "+": floatResult = floatLeft + floatRight; break;
                            case "-": floatResult = floatLeft - floatRight; break;
                            case "*": floatResult = floatLeft * floatRight; break;
                            case "/": floatResult = floatLeft / floatRight; break;
                            case "%":
                                if (floatRight < 0)
                                {
                                    floatRight = -floatRight;
                                    floatResult = floatLeft % floatRight;
                                    if (floatResult > 0) floatResult -= floatRight;
                                }
                                else
                                {
                                    floatResult = floatLeft % floatRight;
                                    if (floatResult < 0) floatResult += floatRight;
                                }
                                break;
                            default:
                                ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                                break;
                        }
                        return Expression.createFloatConstant(opToken, floatResult);

                    case (int)ExpressionType.STRING_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.INTEGER_CONST:
                    case (int)ExpressionType.INTEGER_CONST * (int)ExpressionType.MAX_VALUE + (int)ExpressionType.STRING_CONST:
                        if (op == "*")
                        {
                            Expression strExpr = expr.left.type == ExpressionType.STRING_CONST ? expr.left : expr.right;
                            Expression intExpr = expr.left.type == ExpressionType.INTEGER_CONST ? expr.left : expr.right;
                            int size = intExpr.intVal;
                            string val = strExpr.strVal;
                            if (size == 0) return Expression.createStringConstant(expr.firstToken, "");
                            if (size == 1) return strExpr;
                            if (val.Length * size < 12)
                            {
                                List<string> sb = new List<string>();
                                for (int i = 0; i < size; i++)
                                {
                                    sb.Add(val);
                                }
                                return Expression.createStringConstant(expr.firstToken, string.Join("", sb));
                            }
                            return expr;
                        }

                        ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        break;

                    default:
                        ThrowOpNotDefinedError(opToken, op, expr.left.type, expr.right.type);
                        break;
                }
                throw new InvalidOperationException(); // all code paths above cause an exit.
            }

            return expr;
        }

        private static string GetStringFromConstantExpression(Expression expr)
        {
            if (expr.type == ExpressionType.STRING_CONST) return expr.strVal;
            if (expr.type == ExpressionType.INTEGER_CONST) return expr.intVal + "";
            if (expr.type == ExpressionType.BOOL_CONST) return expr.boolVal ? "true" : "false";
            if (expr.type == ExpressionType.NULL_CONST) return "null"; // TODO: hmm...
            if (expr.type == ExpressionType.ENUM_CONST) return expr.strVal;

            if (expr.type == ExpressionType.FLOAT_CONST)
            {
                string val = expr.floatVal + "";
                if (val.ToLowerInvariant().Contains('e'))
                {
                    throw new NotImplementedException();
                }
                if (val.Contains(',')) val = val.Replace(',', '.');
                if (!val.Contains('.')) val += ".0";
                return val;
            }

            throw new NotImplementedException();
        }

        private Expression SecondPass_ConstructorReference(Expression ctorRef, bool isExpected)
        {
            if (isExpected)
            {
                ctorRef.root.boolVal = true;
                ctorRef.root = this.ResolveExpressionSecondPass(ctorRef.root);
                if (ctorRef.root.type != ExpressionType.CLASS_REFERENCE) Errors.ThrowError(ctorRef.root.firstToken, "This is not a valid class definition.");
                ctorRef.objPtr = ctorRef.root.objPtr;
                ctorRef.root = null;
                return ctorRef;
            }

            Errors.ThrowError(ctorRef.firstToken, "A constructor must be immediately invoked.");
            return ctorRef;
        }

        private Expression SecondPass_BitwiseNot(Expression bwn)
        {
            bwn.root = this.ResolveExpressionSecondPass(bwn.root);

            if (Resolver.IsExpressionConstant(bwn.root))
            {
                if (bwn.root.type != ExpressionType.INTEGER_CONST)
                {
                    Errors.ThrowError(bwn.firstToken, "Bitwise-NOT operator can only be applied on integers.");
                }
                return Expression.createIntegerConstant(bwn.firstToken, ~bwn.root.intVal);
            }
            return bwn;
        }

        private Expression SecondPass_BoolConst(Expression bc)
        {
            return bc;
        }

        private Expression SecondPass_BoolNot(Expression bn)
        {
            bn.root = this.ResolveExpressionSecondPass(bn.root);

            if (Resolver.IsExpressionConstant(bn.root))
            {
                if (bn.root.type != ExpressionType.BOOLEAN_NOT)
                {
                    Errors.ThrowError(bn.firstToken, "Boolean-NOT operator can only be applied to booleans.");
                }
                return Expression.createBoolConstant(bn.firstToken, !bn.root.boolVal);
            }

            return bn;
        }

        private Expression SecondPass_ClassReference(Expression classRef)
        {
            if (!classRef.boolVal)
            {
                Errors.ThrowError(classRef.firstToken, "A class reference must have a field or method referenced from it.");
            }
            return classRef;
        }

        private Expression SecondPass_DictionaryDefinition(Expression dictDef)
        {
            int length = dictDef.keys.Length;
            Dictionary<string, bool> strCollide = new Dictionary<string, bool>();
            Dictionary<int, bool> intCollide = new Dictionary<int, bool>();
            for (int i = 0; i < length; i++)
            {
                Expression key = this.ResolveExpressionSecondPass(dictDef.keys[i]);
                bool isMixed = false;
                bool isCollide = false;
                if (key.type == ExpressionType.INTEGER_CONST)
                {
                    isMixed = strCollide.Count > 0;
                    isCollide = intCollide.ContainsKey(key.intVal);
                    intCollide[key.intVal] = true;
                }
                else if (key.type == ExpressionType.STRING_CONST)
                {
                    isMixed = intCollide.Count > 0;
                    isCollide = strCollide.ContainsKey(key.strVal);
                    strCollide[key.strVal] = true;
                }
                else
                {
                    Errors.ThrowError(key.firstToken, "This type of expression cannot be used as a dictionary key. Dictionary keys must be constant integer or string expressions.");
                }

                if (isMixed)
                {
                    Errors.ThrowError(key.firstToken, "Dictionary cannot contain mixed types for keys.");
                }

                if (isCollide)
                {
                    Errors.ThrowError(key.firstToken, "There are multiple keys with this same value.");
                }

                dictDef.keys[i] = key;
                dictDef.values[i] = this.ResolveExpressionSecondPass(dictDef.values[i]);
            }
            return dictDef;
        }

        private Expression SecondPass_DotField(Expression df)
        {
            // This is the only way to refer to a class reference.
            if (df.root.type == ExpressionType.CLASS_REFERENCE) df.root.boolVal = true;

            df.root = this.ResolveExpressionSecondPass(df.root);
            switch (df.root.type)
            {
                case ExpressionType.STRING_CONST:
                    if (df.strVal == "length")
                    {
                        return Expression.createIntegerConstant(df.firstToken, df.root.strVal.Length);
                    }
                    break;

                case ExpressionType.THIS:
                case ExpressionType.BASE:
                    // TODO: check to see if field exists.
                    // For now, just trust it at runtime as if it were a blind variable.
                    break;

                case ExpressionType.CLASS_REFERENCE:
                    ClassEntity classDef = (ClassEntity)df.root.objPtr;
                    AbstractEntity member = null;
                    if (classDef.classMembers.ContainsKey(df.strVal))
                    {
                        member = classDef.classMembers[df.strVal];
                        if (!member.isStatic)
                        {
                            Errors.ThrowError(df.opToken, classDef.fqName + "." + df.strVal + " is not static.");
                        }
                    }
                    else
                    {
                        Errors.ThrowError(df.opToken, "The class " + classDef.fqName + " does not have a member named '." + df.strVal + "'.");
                    }
                    break;

                default:
                    break;
            }

            return df;
        }

        private Expression SecondPass_EnumConstant(Expression enumConst)
        {
            return enumConst;
        }

        private Expression SecondPass_ExtensionInvocation(Expression expr)
        {
            this.ResolveExpressionArraySecondPass(expr.args);
            int argc = -1;
            if (SpecialActionUtil.IsSpecialActionAndNotExtension(expr.strVal))
            {
                argc = SpecialActionUtil.GetSpecialActionArgc(expr.strVal);
            }
            else
            {
                switch (expr.strVal)
                {
                    case "delay_invoke": argc = 2; break;
                    case "io_stdout": argc = 1; break;
                    case "sleep": argc = 1; break;

                    default:
                        if (this.resolver.isValidRegisteredExtension(expr.strVal))
                        {
                            return expr;
                        }
                        Errors.ThrowError(expr.firstToken, "Extension is not registered: " + expr.strVal);
                        break;
                }
            }

            if (argc != -1 && expr.args.Length != argc)
            {
                Errors.ThrowError(expr.firstToken, "Incorrect number of arguments to extension");
            }

            return expr;
        }

        private Expression SecondPass_FloatConstant(Expression floatConst)
        {
            return floatConst;
        }

        private Expression SecondPass_FunctionInvocation(Expression funcInvoke)
        {
            if (funcInvoke.root.type == ExpressionType.CONSTRUCTOR_REFERENCE)
            {
                Expression ctorRef = this.SecondPass_ConstructorReference(funcInvoke.root, true);
                if (ctorRef.type != ExpressionType.CONSTRUCTOR_REFERENCE) throw new InvalidOperationException(); // this shouldn't happen. 

                this.ResolveExpressionArraySecondPass(funcInvoke.args);

                return Expression.createConstructorInvocation(funcInvoke.firstToken, (AbstractEntity)ctorRef.objPtr, funcInvoke.opToken, funcInvoke.args);
            }

            funcInvoke.root = this.ResolveExpressionSecondPass(funcInvoke.root);

            switch (funcInvoke.root.type)
            {
                case ExpressionType.VARIABLE:
                case ExpressionType.BASE:
                case ExpressionType.FUNCTION_REFERENCE:
                case ExpressionType.DOT_FIELD:
                case ExpressionType.BASE_CTOR_REFERENCE:
                case ExpressionType.INDEX:
                    break;

                default:
                    Errors.ThrowError(funcInvoke.opToken, "Cannot invoke this type of expression like a function.");
                    break;
            }

            this.ResolveExpressionArraySecondPass(funcInvoke.args);

            return funcInvoke;
        }

        private Expression SecondPass_FunctionReference(Expression funcRef)
        {
            return funcRef;
        }

        private Expression SecondPass_ImportReference(Expression importRef)
        {
            Errors.ThrowError(importRef.firstToken, "An import reference cannot be passed as a reference. You must reference the imported entity directly.");
            return null;
        }

        private Expression SecondPass_Index(Expression indexExpr)
        {
            indexExpr.root = this.ResolveExpressionSecondPass(indexExpr.root);
            indexExpr.right = this.ResolveExpressionSecondPass(indexExpr.right);
            return indexExpr;
        }

        private Expression SecondPass_InlineIncrement(Expression inlineIncr)
        {
            inlineIncr.root = this.ResolveExpressionSecondPass(inlineIncr.root);
            switch (inlineIncr.root.type)
            {
                case ExpressionType.VARIABLE:
                case ExpressionType.INDEX:
                case ExpressionType.DOT_FIELD:
                    // these are fine
                    break;

                default:
                    Errors.ThrowError(inlineIncr.opToken, "Cannot use the '" + inlineIncr.opToken.Value + "' operator on this type of expression.");
                    break;
            }
            return inlineIncr;
        }

        private Expression SecondPass_IntegerConstant(Expression intConst)
        {
            return intConst;
        }

        private Expression SecondPass_Lambda(Expression lambda)
        {
            return lambda;
        }

        private Expression SecondPass_ListDefinition(Expression listDef)
        {
            for (int i = 0; i < listDef.values.Length; i++)
            {
                listDef.values[i] = this.ResolveExpressionSecondPass(listDef.values[i]);
            }
            return listDef;
        }

        private Expression SecondPass_NamespaceReference(Expression nsRef)
        {
            Errors.ThrowError(nsRef.firstToken, "You cannot use a namespace reference like this.");
            return null;
        }

        private Expression SecondPass_NegativeSign(Expression negSign)
        {
            Expression root = this.ResolveExpressionSecondPass(negSign.root);
            negSign.root = root;
            if (IsExpressionNumericConstant(root))
            {
                switch (root.type)
                {
                    case ExpressionType.INTEGER_CONST:
                        root.intVal *= -1;
                        break;
                    case ExpressionType.FLOAT_CONST:
                        root.floatVal *= -1;
                        break;
                    case ExpressionType.ENUM_CONST:
                        root = Expression.createIntegerConstant(root.firstToken, -root.intVal);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return root;
            }

            return negSign;
        }

        private Expression SecondPass_NullConstant(Expression nullConst)
        {
            return nullConst;
        }

        private Expression SecondPass_Slice(Expression slice)
        {
            slice.root = this.ResolveExpressionSecondPass(slice.root);
            for (int i = 0; i < 3; i++)
            {
                if (slice.args[i] != null)
                {
                    Expression expr = this.ResolveExpressionSecondPass(slice.args[i]);
                    slice.args[i] = expr;
                    if (Resolver.IsExpressionConstant(expr))
                    {
                        if (expr.type != ExpressionType.INTEGER_CONST && expr.type != ExpressionType.ENUM_CONST)
                        {
                            Errors.ThrowError(expr.firstToken, "Only integers may be used in a slice expression.");
                        }
                    }
                }
            }

            return slice;
        }

        private Expression SecondPass_StringConstant(Expression strConst)
        {
            return strConst;
        }

        private Expression SecondPass_Ternary(Expression ternary)
        {
            ternary.root = this.ResolveExpressionSecondPass(ternary.root);
            ternary.left = this.ResolveExpressionSecondPass(ternary.left);
            ternary.right = this.ResolveExpressionSecondPass(ternary.right);

            if (Resolver.IsExpressionConstant(ternary.root))
            {
                if (ternary.root.type != ExpressionType.BOOL_CONST)
                {
                    Errors.ThrowError(ternary.root.firstToken, "Only booleans can be used as ternary conditions.");
                }

                return ternary.root.boolVal ? ternary.left : ternary.right;
            }

            return ternary;
        }

        private Expression SecondPass_ThisConstant(Expression thisExpr)
        {
            if (this.resolver.activeEntity.nestParent == null) throw new NotImplementedException();
            return thisExpr;
        }

        private Expression SecondPass_TypeOf(Expression typeofExpr)
        {
            typeofExpr.root = this.ResolveExpressionSecondPass(typeofExpr.root);
            string stringConst = null;
            switch (typeofExpr.root.type)
            {
                case ExpressionType.INTEGER_CONST: stringConst = "int"; break;
                case ExpressionType.FLOAT_CONST: stringConst = "float"; break;
                case ExpressionType.NULL_CONST: stringConst = "null"; break;
                case ExpressionType.BOOL_CONST: stringConst = "bool"; break;
                case ExpressionType.STRING_CONST: stringConst = "string"; break;
                case ExpressionType.FUNCTION_REFERENCE: stringConst = "function"; break;

                case ExpressionType.LIST_DEFINITION:
                    if (typeofExpr.root.values.Length == 0) stringConst = "list";
                    break;

                case ExpressionType.DICTIONARY_DEFINITION:
                    if (typeofExpr.root.keys.Length == 0) stringConst = "dict";
                    break;
            }

            if (stringConst != null)
            {
                return Expression.createStringConstant(typeofExpr.firstToken, stringConst);
            }

            return typeofExpr;
        }

        private Expression SecondPass_Variable(Expression varExpr)
        {
            if (varExpr.strVal == "print") throw new InvalidOperationException();
            if (((FunctionLikeEntity)this.resolver.activeEntity).variableScope.ContainsKey(varExpr.strVal)) return varExpr;

            // TODO: come up with a list of suggestions.
            Errors.ThrowError(varExpr.firstToken, "There is no variable by the name of '" + varExpr.strVal + "'.");
            return null;
        }

        private Expression IntegerRequired(Expression expr)
        {
            switch (expr.type)
            {
                case ExpressionType.ENUM_CONST:
                    EnumEntity enumParent = (EnumEntity)expr.objPtr;
                    string enumMem = expr.strVal;
                    for (int i = 0; i < enumParent.memberValues.Length; i++)
                    {
                        if (enumMem == enumParent.memberNameTokens[i].Value)
                        {
                            Expression val = enumParent.memberValues[i];
                            if (val.type != ExpressionType.INTEGER_CONST)
                            {
                                throw new InvalidOperationException();
                            }
                            return val;
                        }
                    }
                    Errors.ThrowError(expr.firstToken, "The enum '" + enumParent.fqName + "' does not have a member named '" + enumMem + "'.");
                    break;

                case ExpressionType.BOOL_CONST:
                case ExpressionType.FLOAT_CONST:
                case ExpressionType.STRING_CONST:
                case ExpressionType.BOOLEAN_NOT:
                    Errors.ThrowError(expr.firstToken, "An integer is expected here.");
                    break;
            }
            return expr;
        }
    }
}
