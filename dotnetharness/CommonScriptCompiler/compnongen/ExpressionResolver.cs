using System;
using System.Collections.Generic;
using CommonScript.Compiler.Internal;

namespace CommonScript.Compiler
{
    internal static class ExpressionResolverUtil
    {
        public static Expression ExpressionResolver_ResolveExpressionFirstPass(Resolver resolver, Expression expr)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.BASE_CTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_FirstPass_BaseCtorReference(resolver, expr);
                case (int) ExpressionType.BINARY_OP: return FunctionWrapper.ExpressionResolver_FirstPass_BinaryOp(resolver, expr);
                case (int) ExpressionType.BITWISE_NOT: return FunctionWrapper.ExpressionResolver_FirstPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return FunctionWrapper.ExpressionResolver_FirstPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_INVOCATION: return FunctionWrapper.ExpressionResolver_FirstPass_ConstructorInvocation(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_FirstPass_ConstructorReference(resolver, expr);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return FunctionWrapper.ExpressionResolver_FirstPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return FunctionWrapper.ExpressionResolver_FirstPass_DotField(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return FunctionWrapper.ExpressionResolver_FirstPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.INDEX: return FunctionWrapper.ExpressionResolver_FirstPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return FunctionWrapper.ExpressionResolver_FirstPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_FirstPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return FunctionWrapper.ExpressionResolver_FirstPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return FunctionWrapper.ExpressionResolver_FirstPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_NullConst(resolver, expr);
                case (int) ExpressionType.SLICE: return FunctionWrapper.ExpressionResolver_FirstPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_FirstPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return FunctionWrapper.ExpressionResolver_FirstPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_FirstPass_This(resolver, expr);
                case (int) ExpressionType.TYPEOF: return FunctionWrapper.ExpressionResolver_FirstPass_TypeOf(resolver, expr);
                case (int) ExpressionType.VARIABLE: return FunctionWrapper.ExpressionResolver_FirstPass_Variable(resolver, expr);

                case (int) ExpressionType.EXTENSION_REFERENCE:
                    FunctionWrapper.Errors_Throw(expr.firstToken, "Extension method references must be invoked immediately.");
                    break;

                default:
                    FunctionWrapper.Errors_ThrowNotImplemented(expr.firstToken, "first pass for this type");
                    break;
            }
            return expr;
        }

        public static Expression ExpressionResolver_ResolveExpressionSecondPass(Resolver resolver, Expression expr)
        {
            switch (expr.type)
            {
                case (int) ExpressionType.BASE_CTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_BaseCtorReference(resolver, expr);
                case (int) ExpressionType.BINARY_OP: return FunctionWrapper.ExpressionResolver_SecondPass_BinaryOp(resolver, expr);
                case (int) ExpressionType.BITWISE_NOT: return FunctionWrapper.ExpressionResolver_SecondPass_BitwiseNot(resolver, expr);
                case (int) ExpressionType.BOOL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_BoolConst(resolver, expr);
                case (int) ExpressionType.BOOLEAN_NOT: return FunctionWrapper.ExpressionResolver_SecondPass_BoolNot(resolver, expr);
                case (int) ExpressionType.CLASS_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ClassReference(resolver, expr);
                case (int) ExpressionType.CONSTRUCTOR_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ConstructorReference(resolver, expr, false);
                case (int) ExpressionType.DICTIONARY_DEFINITION: return FunctionWrapper.ExpressionResolver_SecondPass_DictionaryDefinition(resolver, expr);
                case (int) ExpressionType.DOT_FIELD: return FunctionWrapper.ExpressionResolver_SecondPass_DotField(resolver, expr);
                case (int) ExpressionType.ENUM_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_EnumConstant(resolver, expr);
                case (int) ExpressionType.EXTENSION_INVOCATION: return FunctionWrapper.ExpressionResolver_SecondPass_ExtensionInvocation(resolver, expr);
                case (int) ExpressionType.FLOAT_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_FloatConstant(resolver, expr);
                case (int) ExpressionType.FUNCTION_INVOKE: return FunctionWrapper.ExpressionResolver_SecondPass_FunctionInvocation(resolver, expr);
                case (int) ExpressionType.FUNCTION_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_FunctionReference(resolver, expr);
                case (int) ExpressionType.IMPORT_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_ImportReference(resolver, expr);
                case (int) ExpressionType.INDEX: return FunctionWrapper.ExpressionResolver_SecondPass_Index(resolver, expr);
                case (int) ExpressionType.INLINE_INCREMENT: return FunctionWrapper.ExpressionResolver_SecondPass_InlineIncrement(resolver, expr);
                case (int) ExpressionType.INTEGER_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_IntegerConstant(resolver, expr);
                case (int) ExpressionType.LAMBDA: return FunctionWrapper.ExpressionResolver_SecondPass_Lambda(resolver, expr);
                case (int) ExpressionType.LIST_DEFINITION: return FunctionWrapper.ExpressionResolver_SecondPass_ListDefinition(resolver, expr);
                case (int) ExpressionType.NAMESPACE_REFERENCE: return FunctionWrapper.ExpressionResolver_SecondPass_NamespaceReference(resolver, expr);
                case (int) ExpressionType.NEGATIVE_SIGN: return FunctionWrapper.ExpressionResolver_SecondPass_NegativeSign(resolver, expr);
                case (int) ExpressionType.NULL_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_NullConstant(resolver, expr);
                case (int) ExpressionType.SLICE: return FunctionWrapper.ExpressionResolver_SecondPass_Slice(resolver, expr);
                case (int) ExpressionType.STRING_CONST: return FunctionWrapper.ExpressionResolver_SecondPass_StringConstant(resolver, expr);
                case (int) ExpressionType.TERNARY: return FunctionWrapper.ExpressionResolver_SecondPass_Ternary(resolver, expr);
                case (int) ExpressionType.THIS: return FunctionWrapper.ExpressionResolver_SecondPass_ThisConstant(resolver, expr);
                case (int) ExpressionType.TYPEOF: return FunctionWrapper.ExpressionResolver_SecondPass_TypeOf(resolver, expr);
                case (int) ExpressionType.VARIABLE: return FunctionWrapper.ExpressionResolver_SecondPass_Variable(resolver, expr);
                default:
                    FunctionWrapper.Errors_ThrowNotImplemented(expr.firstToken, "second pass for this type");
                    break;
            }
            return expr;
        }
    }
}
