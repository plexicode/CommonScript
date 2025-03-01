(() => {
const CommonScriptCompiler = (() => {

  const PST = (() => {
    //
const [PASTEL_regCallback, _Errors_ThrowImpl, AbstractEntity_new, AddImplicitIncrementingEnumValueDefinitions, allocateStringAndTokenIds, AttachEntityToParseTree, bsbFlatten, bsbFrom4Bytes, bsbFromBytes, bsbFromInt, bsbFromLenString, bsbFromUtf8String, bsbJoin2, bsbJoin3, bsbJoin4, bsbJoin5, bsbJoin8, BuildFakeDotChain, bundleClass, BundleClassInfo_new, bundleCompilation, bundleEntity, bundleEnum, BundleEnumInfo_createFromEntity, bundleFunction, BundleFunctionInfo_new, ByteCodeBuffer_from2, ByteCodeBuffer_fromRow, ByteCodeRow_new, ByteCodeUtil_ensureBooleanExpression, ByteCodeUtil_ensureIntegerExpression, CatchChunk_new, ClassEntity_new, ClassSorter_calcDepth, ClassSorter_SortClassesInDeterministicDependencyOrder, CompilationBundle_new, CompiledModule_AddLambdas, CompiledModule_InitializeLookups, CompiledModule_new, CompiledModuleEntityLookup, CompilerContext_CalculateCompilationOrder, CompilerContext_CompileModule, CompilerContext_new, ConstEntity_new, convertToBuffer, create0, create1, create2, create3, createFakeToken, createFakeTokenFromTemplate, DotField_getVariableRootedDottedChain, Entity_getMemberLookup, EntityParser_ClassifyToken, EntityResolver_ConvertFieldDefaultValueIntoSetter, EntityResolver_DetermineMemberOffsets, EntityResolver_GetNextAutoVarId, EntityResolver_ResetAutoVarId, EntityResolver_ResolveFunctionFirstPass, EntityResolver_ResolveFunctionSecondPass, EnumEntity_new, Errors_Throw, Errors_ThrowEof, Errors_ThrowGeneralError, Errors_ThrowNotImplemented, ExportUtil_exportBundle, ExportUtil_exportCode, Expression_cloneWithNewToken, Expression_createBaseCtorReference, Expression_createBaseReference, Expression_createBinaryOp, Expression_createBoolConstant, Expression_createBracketIndex, Expression_createClassReference, Expression_createConstructorInvocation, Expression_createConstructorReference, Expression_createDictionaryDefinition, Expression_createDotField, Expression_createEnumConstant, Expression_createEnumReference, Expression_createExtensionInvocation, Expression_createExtensionReference, Expression_createFloatConstant, Expression_createFunctionInvocation, Expression_createFunctionReference, Expression_createImportReference, Expression_createInlineIncrement, Expression_createIntegerConstant, Expression_createLambda, Expression_createListDefinition, Expression_createNamespaceReference, Expression_createNegatePrefix, Expression_createNullConstant, Expression_createSliceExpression, Expression_createStringConstant, Expression_createTernary, Expression_createThisReference, Expression_createTypeof, Expression_createVariable, Expression_new, ExpressionResolver_FindLocallyReferencedEntity, ExpressionResolver_FirstPass_BaseCtorReference, ExpressionResolver_FirstPass_BinaryOp, ExpressionResolver_FirstPass_BitwiseNot, ExpressionResolver_FirstPass_BoolConst, ExpressionResolver_FirstPass_BoolNot, ExpressionResolver_FirstPass_ConstructorInvocation, ExpressionResolver_FirstPass_ConstructorReference, ExpressionResolver_FirstPass_DictionaryDefinition, ExpressionResolver_FirstPass_DotField, ExpressionResolver_FirstPass_FloatConstant, ExpressionResolver_FirstPass_FunctionInvocation, ExpressionResolver_FirstPass_Index, ExpressionResolver_FirstPass_InlineIncrement, ExpressionResolver_FirstPass_IntegerConstant, ExpressionResolver_FirstPass_Lambda, ExpressionResolver_FirstPass_ListDefinition, ExpressionResolver_FirstPass_NegativeSign, ExpressionResolver_FirstPass_NullConst, ExpressionResolver_FirstPass_Slice, ExpressionResolver_FirstPass_StringConstant, ExpressionResolver_FirstPass_Ternary, ExpressionResolver_FirstPass_This, ExpressionResolver_FirstPass_TypeOf, ExpressionResolver_FirstPass_Variable, ExpressionResolver_IntegerRequired, ExpressionResolver_ResolveExpressionArrayFirstPass, ExpressionResolver_ResolveExpressionArraySecondPass, ExpressionResolver_ResolveExpressionFirstPass, ExpressionResolver_ResolveExpressionSecondPass, ExpressionResolver_SecondPass_BaseCtorReference, ExpressionResolver_SecondPass_BinaryOp, ExpressionResolver_SecondPass_BitwiseNot, ExpressionResolver_SecondPass_BoolConst, ExpressionResolver_SecondPass_BoolNot, ExpressionResolver_SecondPass_ClassReference, ExpressionResolver_SecondPass_ConstructorReference, ExpressionResolver_SecondPass_DictionaryDefinition, ExpressionResolver_SecondPass_DotField, ExpressionResolver_SecondPass_EnumConstant, ExpressionResolver_SecondPass_ExtensionInvocation, ExpressionResolver_SecondPass_FloatConstant, ExpressionResolver_SecondPass_FunctionInvocation, ExpressionResolver_SecondPass_FunctionReference, ExpressionResolver_SecondPass_ImportReference, ExpressionResolver_SecondPass_Index, ExpressionResolver_SecondPass_InlineIncrement, ExpressionResolver_SecondPass_IntegerConstant, ExpressionResolver_SecondPass_Lambda, ExpressionResolver_SecondPass_ListDefinition, ExpressionResolver_SecondPass_NamespaceReference, ExpressionResolver_SecondPass_NegativeSign, ExpressionResolver_SecondPass_NullConstant, ExpressionResolver_SecondPass_Slice, ExpressionResolver_SecondPass_StringConstant, ExpressionResolver_SecondPass_Ternary, ExpressionResolver_SecondPass_ThisConstant, ExpressionResolver_SecondPass_TypeOf, ExpressionResolver_SecondPass_Variable, ExpressionResolver_WrapEntityIntoReferenceExpression, fail, FieldEntity_new, FileContext_initializeImportLookup, FileContext_new, finalizeBreakContinue, flatten, FlattenBinaryOpChain, FlattenEntities, FloatToStringWorkaround, FunctionEntity_BuildConstructor, FunctionEntity_BuildLambda, FunctionEntity_BuildMethodOrStandalone, FunctionEntity_new, GEN_BUILTINS_base64, GEN_BUILTINS_builtins, GEN_BUILTINS_json, GEN_BUILTINS_math, GEN_BUILTINS_random, GEN_BUILTINS_textencoding, GEN_BUILTINS_xml, GetBuiltinRawStoredString, getDeterministOrderOfModules, GetNumericValueOfConstantExpression, GetSourceForBuiltinModule, GetStringFromConstantExpression, ImportParser_AdvanceThroughImports, ImportParser_createBuiltinImport, ImportStatement_new, IsBuiltInModule, IsExpressionConstant, IsExpressionNumericConstant, join2, join3, join4, join5, join6, join7, LookupUtil_DoFirstPassVariableLookupThroughImports, LookupUtil_DoLookupForName, LookupUtil_tryCreateModuleMemberReference, ModuleWrapperEntity_new, NamespaceEntity_new, OrderStringsByDescendingFrequencyUsingLookup, PadIntegerToSize, ParseAddition, ParseAnnotations, ParseAnyForLoop, ParseArgDefinitionList, ParseAtomicExpression, ParseBitshift, ParseBitwise, ParseBooleanCombination, ParseBreakContinue, ParseClass, ParseCodeBlock, ParseCodeBlockList, ParseConst, ParseConstructor, ParseDictionaryDefinition, ParseDoWhileLoop, ParseEnum, ParseEquality, ParseExponent, ParseExpression, ParseField, ParseForEachLoop, ParseFunctionDefinition, ParseIfStatement, ParseInequality, ParseInlineIncrementPrefix, ParseLambda, ParseListDefinition, ParseMultiplication, ParseNamespace, ParseNegatePrefix, ParseNullCoalesce, ParseOutEntities, ParseReturn, ParseStatement, ParseSwitch, ParseTernary, ParseThrow, ParseTraditionalForLoop, ParseTry, ParseTypeofPrefix, ParseUnaryPrefix, ParseUnarySuffix, ParseWhileLoop, PerformFullResolutionPassOnConstAndEnums, PUBLIC_CompleteCompilation, PUBLIC_EnsureDependenciesFulfilled, PUBLIC_GetNextRequiredModuleId, PUBLIC_getTokenErrPrefix, PUBLIC_SupplyFilesForModule, Resolve, ResolveBaseClassesAndEstablishClassOrder, Resolver_DetermineConstAndEnumResolutionOrder, Resolver_GetEnumMemberIndex, Resolver_GetListOfUnresolvedConstReferences, Resolver_GetListOfUnresolvedConstReferencesImpl, Resolver_isValidRegisteredExtension, Resolver_new, Resolver_ReportNewLambda, serializeAssignField, serializeAssignIndex, serializeAssignVariable, serializeBaseCtorReference, serializeBinaryOp, serializeBitwiseNot, serializeBoolConst, serializeBooleanNot, serializeBreak, serializeClassReference, serializeCodeBlock, serializeConstructorInvocation, serializeContinue, serializeDictionaryDefinition, serializeDotField, serializeDoWhileLoop, serializeExpression, serializeExpressionStatement, serializeExtensionInvocation, serializeFloatConstant, serializeForEachLoop, serializeForLoop, serializeFunctionInvocation, serializeFunctionReference, serializeIfStatement, serializeIndex, serializeInlineIncrement, serializeInlineIncrementDotField, serializeInlineIncrementIndex, serializeInlineIncrementVar, serializeIntegerConstant, serializeLambda, serializeListDefinition, serializeNegativeSign, serializeNullConstant, serializeReturn, serializeSlice, serializeSpecialAction, serializeStatement, serializeStringConstant, serializeSwitchStatement, serializeTernary, serializeThis, serializeThrowStatement, serializeTryStatement, serializeTypeOf, serializeVariable, serializeWhileLoop, SimpleExprToTypeName, SpecialActionUtil_GetSpecialActionArgc, SpecialActionUtil_GetSpecialActionOpCode, SpecialActionUtil_IsSpecialActionAndNotExtension, SpecialActionUtil_new, Statement_createAssignment, Statement_createBreakContinue, Statement_createDoWhile, Statement_createExpressionAsStatement, Statement_createForEachLoop, Statement_createForLoop, Statement_createIfStatement, Statement_createReturn, Statement_createSwitchStatement, Statement_createThrow, Statement_createTry, Statement_createWhileLoop, Statement_new, StatementParser_IdentifyKeywordType, StatementResolver_FirstPass_Assignment, StatementResolver_FirstPass_Break, StatementResolver_FirstPass_Continue, StatementResolver_FirstPass_DoWhileLoop, StatementResolver_FirstPass_ForEachLoop, StatementResolver_FirstPass_ForLoop, StatementResolver_FirstPass_IfStatement, StatementResolver_FirstPass_SwitchStatement, StatementResolver_FirstPass_Try, StatementResolver_FirstPass_WhileLoop, StatementResolver_ResolveStatementArrayFirstPass, StatementResolver_ResolveStatementArraySecondPass, StatementResolver_ResolveStatementFirstPass, StatementResolver_ResolveStatementSecondPass, StatementResolver_SecondPass_Assignment, StatementResolver_SecondPass_Break, StatementResolver_SecondPass_Continue, StatementResolver_SecondPass_DoWhileLoop, StatementResolver_SecondPass_ExpressionAsStatement, StatementResolver_SecondPass_ForEachLoop, StatementResolver_SecondPass_ForLoop, StatementResolver_SecondPass_IfStatement, StatementResolver_SecondPass_Return, StatementResolver_SecondPass_SwitchStatement, StatementResolver_SecondPass_ThrowStatement, StatementResolver_SecondPass_TryStatement, StatementResolver_SecondPass_WhileLoop, StaticContext_new, StringArraySlice, StringArrayToList, StringSet_add, StringSet_fromArray, StringSet_fromList, StringSet_has, StringSet_new, StringToUnicodeArray, SwitchChunk_new, ThrowOpNotDefinedError, Token_getFingerprint, Token_new, Tokenize, TokenizerStaticContext_new, Tokens_doesNextInclude5, Tokens_doesNextInclulde2, Tokens_doesNextInclulde3, Tokens_doesNextInclulde4, Tokens_ensureMore, Tokens_hasMore, Tokens_isNext, Tokens_isSequenceNext2, Tokens_isSequenceNext3, Tokens_isSequenceNext4, Tokens_peek, Tokens_peekAhead, Tokens_peekType, Tokens_peekValue, Tokens_peekValueNonNull, Tokens_pop, Tokens_popExpected, Tokens_popIfPresent, Tokens_popKeyword, Tokens_popName, TokenStream_new, TryDoExactLookupForConstantEntity, TryParseFloat, TryParseInteger, TryParseString, TryPopAssignmentOp, UnicodeArrayToString, UnicodeArrayToString_slice] = (() => {
let PST$createNewArray = s => {
	let o = [];
	while (s --> 0) o.push(null);
	return o;
};

let PST$stringToUtf8Bytes = s => Array.from(new TextEncoder().encode(s));

let PST$sortedCopyOfArray = v => {
	let o = [...v];
	if (o.length < 2) return o;
	if (typeof(o[0]) === 'number') return o.sort((a, b) => a - b);
	return o.sort();
};

let PST$clearList = a => {
	while (a.length) a.pop();
};

let PST$floatParseHelper = (o, s) => {
	o[0] = -1;
	let t = parseFloat(s);
	if (isNaN(t) || !isFinite(t)) return;
	o[0] = 1;
	o[1] = t;
};

let PST$extCallbacks = {};

let PST$registerExtensibleCallback = (name, fn) => { PST$extCallbacks[name] = fn; };

let _Errors_ThrowImpl = function(type, t, s1, s2) {
	let args = PST$createNewArray(3);
	args[0] = type;
	if (type == 1) {
		args[1] = t;
		args[2] = s1;
	} else if (type == 2) {
		args[1] = s1;
		args[2] = s2;
	} else if (type == 3) {
		args[1] = s1;
	}
	return (PST$extCallbacks["throwParserException"] || ((o) => null))(args);
};

let AbstractEntity_new = function(firstToken, type, specificData) {
	return [firstToken, type, specificData, null, null, null, null, false, null, null, -1];
};

let AddImplicitIncrementingEnumValueDefinitions = function(enums) {
	let i = 0;
	while (i < enums.length) {
		let enumEnt = enums[i];
		let j = 0;
		while (j < enumEnt[0].length) {
			let token = enumEnt[0][j];
			if (enumEnt[1][j] == null) {
				if (j == 0) {
					enumEnt[1][j] = Expression_createIntegerConstant(token, 1);
				} else {
					enumEnt[1][j] = Expression_createBinaryOp(BuildFakeDotChain(enumEnt[2][3], enumEnt[0][j - 1][0]), createFakeTokenFromTemplate(token, "+", 3), Expression_createIntegerConstant(null, 1));
				}
			}
			j++;
		}
		i++;
	}
};

let allocateStringAndTokenIds = function(bundle) {
	let i = 0;
	let j = 0;
	let allByteCode = [];
	i = 1;
	while (i < bundle[2].length) {
		let fn = bundle[2][i];
		j = 0;
		while (j < fn[0].length) {
			allByteCode.push(fn[0][j]);
			j += 1;
		}
		i += 1;
	}
	i = 1;
	while (i < bundle[3].length) {
		let fn = bundle[3][i];
		j = 0;
		while (j < fn[0].length) {
			allByteCode.push(fn[0][j]);
			j += 1;
		}
		i += 1;
	}
	let stringUsageCount = {};
	let tokenCountByFingerprint = {};
	i = 0;
	while (i < allByteCode.length) {
		let row = allByteCode[i];
		let str = row[1];
		let tok = row[2];
		if (str != null) {
			if (!(stringUsageCount[str] !== undefined)) {
				stringUsageCount[str] = 0;
			}
			stringUsageCount[str] = stringUsageCount[str] + 1;
		}
		if (tok != null) {
			let fp = Token_getFingerprint(tok);
			if (!(tokenCountByFingerprint[fp] !== undefined)) {
				tokenCountByFingerprint[fp] = 0;
			}
			tokenCountByFingerprint[fp] = tokenCountByFingerprint[fp] + 1;
		}
		i += 1;
	}
	let stringByIndex = OrderStringsByDescendingFrequencyUsingLookup(stringUsageCount);
	let fpByIndex = OrderStringsByDescendingFrequencyUsingLookup(tokenCountByFingerprint);
	let stringById = [];
	stringById.push(null);
	i = 0;
	while (i < stringByIndex.length) {
		stringById.push(stringByIndex[i]);
		i += 1;
	}
	let stringToId = {};
	let tokenFingerprintToId = {};
	i = 0;
	while (i < stringByIndex.length) {
		let s = stringByIndex[i];
		stringToId[s] = i + 1;
		i += 1;
	}
	i = 0;
	while (i < fpByIndex.length) {
		let fp = fpByIndex[i];
		tokenFingerprintToId[fp] = i;
		i += 1;
	}
	let tokensById = PST$createNewArray(fpByIndex.length);
	i = 0;
	while (i < allByteCode.length) {
		let row = allByteCode[i];
		if (row[1] != null) {
			row[4] = stringToId[row[1]];
		}
		if (row[2] != null) {
			row[5] = tokenFingerprintToId[row[2][5]];
			tokensById[row[5]] = row[2];
		}
		i += 1;
	}
	bundle[6] = tokensById;
	bundle[0] = [...(stringById)];
};

let AttachEntityToParseTree = function(child, parent, file, activeNsPrefix, activeEntityBucket, annotationTokens) {
	child[9] = file;
	child[6] = annotationTokens;
	let fqName = child[3];
	if (activeNsPrefix != "") {
		fqName = [activeNsPrefix, ".", fqName].join('');
	}
	child[5] = fqName;
	child[8] = parent;
	let isStatic = annotationTokens["static"] !== undefined;
	let isAttachingToClass = parent != null && parent[1] == 1;
	let isClass = child[1] == 1;
	let isCtor = child[1] == 3;
	if (isCtor && !isAttachingToClass) {
		Errors_Throw(child[0], "Cannot place a constructor here. Constructors can only be added to classes.");
	}
	if (isStatic && !isClass && !isAttachingToClass) {
		Errors_Throw(child[0], "@static is not applicable to this type of entity.");
	}
	if (activeEntityBucket[child[3]] !== undefined) {
		Errors_Throw(child[0], ["There are multiple entities named ", child[5], "."].join(''));
	}
	activeEntityBucket[child[3]] = child;
};

let bsbFlatten = function(sbs) {
	let q = [];
	q.push(sbs);
	let output = [];
	while (q.length > 0) {
		let lastIndex = q.length - 1;
		let current = q[lastIndex];
		q.splice(lastIndex, 1);
		if (current[0]) {
			let currentBytes = current[2];
			let length = currentBytes.length;
			let i = 0;
			while (i < length) {
				output.push(currentBytes[i]);
				i += 1;
			}
		} else {
			q.push(current[4]);
			q.push(current[3]);
		}
	}
	return [...(output)];
};

let bsbFrom4Bytes = function(a, b, c, d) {
	let arr = PST$createNewArray(4);
	arr[0] = a;
	arr[1] = b;
	arr[2] = c;
	arr[3] = d;
	return bsbFromBytes(arr);
};

let bsbFromBytes = function(bytes) {
	return [true, bytes.length, bytes, null, null];
};

let bsbFromInt = function(value) {
	let buf = null;
	if (value >= 0 && value < 128) {
		buf = PST$createNewArray(1);
		buf[0] = value;
	} else if (value == -2147483648) {
		buf = PST$createNewArray(1);
		buf[0] = 192;
	} else {
		let firstByte = 128;
		let isNegative = value < 0;
		if (isNegative) {
			value *= -1;
			firstByte |= 16;
		}
		if (value < 256) {
			firstByte |= 1;
			buf = PST$createNewArray(2);
			buf[0] = firstByte;
			buf[1] = value & 255;
		} else if (value <= 65535) {
			firstByte |= 2;
			buf = PST$createNewArray(3);
			buf[0] = firstByte;
			buf[1] = value >> 8 & 255;
			buf[2] = value & 255;
		} else if (value <= 16777215) {
			firstByte |= 3;
			buf = PST$createNewArray(4);
			buf[0] = firstByte;
			buf[1] = value >> 16 & 255;
			buf[2] = value >> 8 & 255;
			buf[3] = value & 255;
		} else if (value <= 2147483647) {
			firstByte |= 4;
			buf = PST$createNewArray(5);
			buf[0] = firstByte;
			buf[1] = value >> 24 & 255;
			buf[2] = value >> 16 & 255;
			buf[3] = value >> 8 & 255;
			buf[4] = value & 255;
		} else {
			fail("Not implemented");
		}
	}
	return [true, buf.length, buf, null, null];
};

let bsbFromLenString = function(value) {
	let payload = bsbFromUtf8String(value);
	return bsbJoin2(bsbFromInt(payload[1]), payload);
};

let bsbFromUtf8String = function(value) {
	let bytes = PST$stringToUtf8Bytes(value);
	return [true, bytes.length, bytes, null, null];
};

let bsbJoin2 = function(a, b) {
	if (a == null) {
		return b;
	}
	if (b == null) {
		return a;
	}
	return [false, a[1] + b[1], null, a, b];
};

let bsbJoin3 = function(a, b, c) {
	return bsbJoin2(bsbJoin2(a, b), c);
};

let bsbJoin4 = function(a, b, c, d) {
	return bsbJoin2(bsbJoin2(a, b), bsbJoin2(c, d));
};

let bsbJoin5 = function(a, b, c, d, e) {
	return bsbJoin3(bsbJoin2(a, b), bsbJoin2(c, d), e);
};

let bsbJoin8 = function(a, b, c, d, e, f, g, h) {
	return bsbJoin2(bsbJoin4(a, b, c, d), bsbJoin4(e, f, g, h));
};

let BuildFakeDotChain = function(root, field) {
	let varRoot = Expression_createVariable(null, root);
	return Expression_createDotField(varRoot, null, field);
};

let bundleClass = function(classEntity, bundle) {
	let baseClassId = 0;
	if (classEntity[1] != null) {
		baseClassId = classEntity[1][6][10];
	}
	let mems = classEntity[5];
	let staticCtorId = 0;
	if (mems["@cctor"] !== undefined) {
		staticCtorId = mems["@cctor"][10];
	}
	let bci = BundleClassInfo_new(classEntity[6][10], baseClassId, classEntity[6][3], mems["@ctor"][10], staticCtorId, {}, classEntity[4], [], []);
	let classMemberNames = Object.keys(classEntity[5]);
	let i = 0;
	while (i < classMemberNames.length) {
		let memberName = classMemberNames[i];
		let member = classEntity[5][memberName];
		switch (member[1]) {
			case 6:
				bci[5][memberName] = member[10];
				if (member[7]) {
					bci[7].push(memberName);
				}
				break;
			case 5:
				if (member[7]) {
					bci[8].push(memberName);
				}
				break;
			case 3:
				break;
			default:
				fail("Not implemented");
				break;
		}
		i += 1;
	}
	bundle[4].push(bci);
};

let BundleClassInfo_new = function(classId, parentId, name, ctorId, staticCtorId, methodsToId, newDirectMembersByNextOffsets, staticMethods, staticFields) {
	return [classId, parentId, name, ctorId, staticCtorId, methodsToId, newDirectMembersByNextOffsets, staticMethods, staticFields];
};

let bundleCompilation = function(staticCtx, rootId, modules) {
	let i = 0;
	let j = 0;
	let deterministicOrder = getDeterministOrderOfModules(modules);
	let bundle = CompilationBundle_new();
	let userFunctions = [];
	let coreBuiltInFunctions = [];
	let enums = [];
	let classes = [];
	let fields = [];
	let lambdas = [];
	let mainFunc = null;
	i = 0;
	while (i < deterministicOrder.length) {
		let m = deterministicOrder[i];
		let deterministicKeyOrder = PST$sortedCopyOfArray(Object.keys(m[3]));
		let checkForMain = m[0] == rootId;
		let orderedEntities = [];
		j = 0;
		while (j < deterministicKeyOrder.length) {
			orderedEntities.push(m[3][deterministicKeyOrder[j]]);
			j += 1;
		}
		j = 0;
		while (j < m[5].length) {
			orderedEntities.push(m[5][j]);
			j += 1;
		}
		j = 0;
		while (j < orderedEntities.length) {
			let tle = orderedEntities[j];
			switch (tle[1]) {
				case 2:
					break;
				case 6:
					let func = tle[2];
					if (tle[9][7]) {
						coreBuiltInFunctions.push(func);
					} else {
						userFunctions.push(func);
						if (checkForMain && tle[3] == "main") {
							if (mainFunc != null) {
								Errors_Throw(tle[0], "There are multiple functions named main in the root module.");
							}
							mainFunc = func;
						}
					}
					break;
				case 10:
					lambdas.push(tle[2]);
					break;
				case 5:
					fields.push(tle[2]);
					break;
				case 4:
					enums.push(tle[2]);
					break;
				case 1:
					classes.push(tle[2]);
					break;
				case 3:
					userFunctions.push(tle[2]);
					break;
				case 7:
					break;
				default:
					fail("Not implemented");
					break;
			}
			j += 1;
		}
		i += 1;
	}
	let finalOrder = [];
	let allFunctions = [];
	i = 0;
	while (i < coreBuiltInFunctions.length) {
		allFunctions.push(coreBuiltInFunctions[i]);
		i += 1;
	}
	i = 0;
	while (i < userFunctions.length) {
		allFunctions.push(userFunctions[i]);
		i += 1;
	}
	i = 0;
	while (i < allFunctions.length) {
		let fn = allFunctions[i][3];
		fn[10] = i + 1;
		finalOrder.push(fn);
		i += 1;
	}
	i = 0;
	while (i < enums.length) {
		enums[i][2][10] = i + 1;
		finalOrder.push(enums[i][2]);
		i += 1;
	}
	let sortedClasses = ClassSorter_SortClassesInDeterministicDependencyOrder([...(classes)], true);
	i = 0;
	while (i < sortedClasses.length) {
		let cls = sortedClasses[i];
		cls[6][10] = i + 1;
		finalOrder.push(cls[6]);
		i += 1;
	}
	i = 0;
	while (i < lambdas.length) {
		lambdas[i][3][10] = i + 1;
		finalOrder.push(lambdas[i][3]);
		i += 1;
	}
	i = 0;
	while (i < finalOrder.length) {
		let entity = finalOrder[i];
		bundleEntity(staticCtx, entity, bundle);
		i += 1;
	}
	allocateStringAndTokenIds(bundle);
	if (mainFunc == null) {
		Errors_ThrowGeneralError("There is no main() function defined.");
	}
	if (mainFunc[0].length >= 2) {
		Errors_Throw(mainFunc[3][4], "The main function can only take in at most one argument. Note that multiple CLI arguments are passed in to main(args) as a single list of strings.");
	}
	bundle[7] = mainFunc[3][10];
	bundle[8] = coreBuiltInFunctions.length;
	return bundle;
};

let bundleEntity = function(staticCtx, entity, bundle) {
	switch (entity[1]) {
		case 2:
			break;
		case 1:
			bundleClass(entity[2], bundle);
			break;
		case 4:
			bundleEnum(entity[2], bundle);
			break;
		case 5:
			fail("not implemented");
			break;
		case 6:
			bundleFunction(staticCtx, entity[2], bundle);
			break;
		case 3:
			bundleFunction(staticCtx, entity[2], bundle);
			break;
		case 10:
			bundleFunction(staticCtx, entity[2], bundle);
			break;
		case 8:
			fail("not implemented");
			break;
		default:
			fail("");
			break;
	}
};

let bundleEnum = function(enumEntity, bundle) {
	let bei = BundleEnumInfo_createFromEntity(enumEntity);
	bundle[5].push(bei);
};

let BundleEnumInfo_createFromEntity = function(e) {
	let sz = e[1].length;
	let names = PST$createNewArray(sz);
	let values = PST$createNewArray(sz);
	let i = 0;
	while (i < sz) {
		names[i] = e[0][i][0];
		values[i] = e[1][i][8];
		i += 1;
	}
	return [e[2][10], names, values];
};

let bundleFunction = function(staticCtx, entity, bundle) {
	let i = 0;
	let isLambda = entity[3][1] == 10;
	let buffer = null;
	let argc = entity[0].length;
	let argcMin = 0;
	i = 0;
	while (i < argc) {
		let argToken = entity[0][i];
		let argValue = entity[1][i];
		let argBuffer = null;
		if (argValue == null) {
			argcMin += 1;
			argBuffer = create1(33, argToken, null, i);
		} else {
			let defaultValBuffer = serializeExpression(staticCtx, argValue);
			argBuffer = create2(34, argToken, null, i, defaultValBuffer[0]);
			argBuffer = join2(argBuffer, defaultValBuffer);
		}
		buffer = join3(buffer, argBuffer, create0(3, argToken, argToken[0]));
		i += 1;
	}
	i = 0;
	while (i < entity[4].length) {
		let stmnt = entity[4][i];
		buffer = join2(buffer, serializeStatement(staticCtx, stmnt));
		i += 1;
	}
	let flatByteCode = flatten(buffer);
	let byteCodeFinal = [];
	i = 0;
	while (i < flatByteCode.length) {
		byteCodeFinal.push(flatByteCode[i]);
		i += 1;
	}
	i = 0;
	while (i < byteCodeFinal.length) {
		let row = byteCodeFinal[i];
		if (row[0] <= 0) {
			fail("");
		}
		if (row[6] != null) {
			let tryInfoArgs = row[6];
			tryInfoArgs[0] = i - byteCodeFinal.length;
			let tryInfoRow = ByteCodeRow_new(62, null, null, tryInfoArgs);
			byteCodeFinal.push(tryInfoRow);
		}
		i += 1;
	}
	let fnName = null;
	if (!isLambda) {
		fnName = entity[3][3];
	}
	let fd = BundleFunctionInfo_new(byteCodeFinal, argcMin, argc, fnName);
	if (isLambda) {
		bundle[3].push(fd);
	} else {
		bundle[2].push(fd);
	}
};

let BundleFunctionInfo_new = function(code, argcMin, argcMax, name) {
	return [[...(code)], argcMin, argcMax, name];
};

let ByteCodeBuffer_from2 = function(left, right) {
	return [left[0] + right[0], false, left, right, null, left[5], right[6]];
};

let ByteCodeBuffer_fromRow = function(row) {
	return [1, true, null, null, row, row, row];
};

let ByteCodeRow_new = function(opCode, token, stringArg, args) {
	return [opCode, stringArg, token, args, 0, 0, null];
};

let ByteCodeUtil_ensureBooleanExpression = function(throwToken, buf) {
	if (buf == null) {
		fail("invalid operation");
		return null;
	}
	let last = buf[6];
	if (last[0] == 4) {
		let op = last[1];
		if (op == "||" || op == "&&" || op == "==" || op == "!=" || op == "<" || op == ">" || op == "<=" || op == ">=") {
			return buf;
		}
	}
	if (last[0] == 16) {
		return buf;
	}
	if (last[0] == 36) {
		return buf;
	}
	if (last[0] == 11) {
		return buf;
	}
	return join2(buf, create0(16, throwToken, null));
};

let ByteCodeUtil_ensureIntegerExpression = function(throwToken, buf) {
	if (buf == null) {
		fail("invalid operation");
		return null;
	}
	let last = buf[6];
	if (last[0] == 40) {
		return buf;
	}
	if (last[0] == 10) {
		return buf;
	}
	return join2(buf, create0(18, throwToken, null));
};

let CatchChunk_new = function(catchCode, exceptionClassNamesRaw, exceptionVarToken) {
	return [[...(exceptionClassNamesRaw)], null, exceptionVarToken, catchCode, false];
};

let ClassEntity_new = function(classToken, nameToken, fqName) {
	let cd = [null, null, null, null, null, {}, null];
	cd[6] = AbstractEntity_new(classToken, 1, cd);
	cd[6][4] = nameToken;
	cd[6][3] = nameToken[0];
	cd[6][5] = fqName;
	return cd;
};

let ClassSorter_calcDepth = function(cls, depthByName) {
	let fqName = cls[6][5];
	if (depthByName[fqName] !== undefined) {
		return depthByName[fqName];
	}
	if (cls[1] == null) {
		depthByName[fqName] = 1;
		return 1;
	}
	let depth = ClassSorter_calcDepth(cls[1], depthByName) + 1;
	depthByName[fqName] = depth;
	return depth;
};

let ClassSorter_SortClassesInDeterministicDependencyOrder = function(unorderedClasses, considerBaseClasses) {
	let i = 0;
	let classDepthByFqName = {};
	let classByLexicalKey = {};
	let cls = null;
	let padSize = (unorderedClasses.length + '').length + 1;
	i = 0;
	while (i < unorderedClasses.length) {
		cls = unorderedClasses[i];
		let depth = 1;
		if (considerBaseClasses) {
			depth = ClassSorter_calcDepth(cls, classDepthByFqName);
		}
		let key = [PadIntegerToSize(depth, padSize), ":", cls[6][5]].join('');
		classByLexicalKey[key] = cls;
		i += 1;
	}
	let keys = PST$sortedCopyOfArray(Object.keys(classByLexicalKey));
	let output = PST$createNewArray(keys.length);
	i = 0;
	while (i < keys.length) {
		output[i] = classByLexicalKey[keys[i]];
		i += 1;
	}
	return output;
};

let CompilationBundle_new = function() {
	let b = [null, null, null, null, null, null, null, 0, 0];
	b[1] = [];
	b[1].push(null);
	b[2] = [];
	b[2].push(null);
	b[4] = [];
	b[4].push(null);
	b[5] = [];
	b[5].push(null);
	b[3] = [];
	b[3].push(null);
	return b;
};

let CompiledModule_AddLambdas = function(m, lambdas) {
	m[5] = [];
	let i = 0;
	while (i < lambdas.length) {
		m[5].push(lambdas[i][3]);
		i += 1;
	}
};

let CompiledModule_InitializeLookups = function(m, rootEntities, flatEntities) {
	m[2] = rootEntities;
	m[3] = flatEntities;
	m[4] = {};
	let fqNames = Object.keys(m[3]);
	let i = 0;
	while (i < fqNames.length) {
		let fqName = fqNames[i];
		let entity = m[3][fqName];
		if (entity[1] == 4) {
			let enumMemberNameTokens = entity[2][0];
			let j = 0;
			while (j < enumMemberNameTokens.length) {
				let enumMem = enumMemberNameTokens[j];
				m[4][[fqName, ".", enumMem[0]].join('')] = entity;
				j += 1;
			}
		} else {
			m[4][fqName] = entity;
		}
		i += 1;
	}
};

let CompiledModule_new = function(id) {
	return [id, {}, null, null, null, null];
};

let CompiledModuleEntityLookup = function(mod, fqName) {
	if (mod[4][fqName] !== undefined) {
		return mod[4][fqName];
	}
	let potentialEnumParentName = StringArraySlice(fqName.split("."), 0, 1).join(".");
	if (mod[4][potentialEnumParentName] !== undefined) {
		return mod[4][potentialEnumParentName];
	}
	return null;
};

let CompilerContext_CalculateCompilationOrder = function(compiler) {
	let recurseState = {};
	let order = [];
	let queue = [];
	queue.push(compiler[1]);
	while (queue.length > 0) {
		let last = queue.length - 1;
		let currentId = queue[last];
		queue.splice(last, 1);
		let currentRecurseState = 0;
		if (recurseState[currentId] !== undefined) {
			currentRecurseState = recurseState[currentId];
		}
		if (currentRecurseState == 2) {
		} else {
			let deps = compiler[2][currentId];
			let newDeps = [];
			let i = 0;
			while (i < deps.length) {
				let depId = deps[i];
				if (recurseState[depId] !== undefined) {
					if (recurseState[depId] == 2) {
					} else if (recurseState[depId] == 1) {
						fail(["There is a cyclical dependency involving ", depId, " and ", currentId].join(''));
					}
				} else {
					newDeps.push(depId);
				}
				i += 1;
			}
			if (newDeps.length == 0) {
				recurseState[currentId] = 2;
				order.push(currentId);
			} else {
				recurseState[currentId] = 1;
				queue.push(currentId);
				i = 0;
				while (i < newDeps.length) {
					queue.push(newDeps[i]);
					i += 1;
				}
			}
		}
	}
	return [...(order)];
};

let CompilerContext_CompileModule = function(compiler, moduleId) {
	let i = 0;
	let files = compiler[3][moduleId];
	let rootEntities = {};
	let sourceCode = {};
	i = 0;
	while (i < files.length) {
		let file = files[i];
		sourceCode[file[1]] = file[2];
		let j = 0;
		while (j < file[4].length) {
			let importStatement = file[4][j];
			importStatement[5] = compiler[6][importStatement[2]];
			j += 1;
		}
		ParseOutEntities(compiler, file, rootEntities, null, "");
		let danglingToken = Tokens_peek(file[3]);
		if (danglingToken != null) {
			Errors_Throw(danglingToken, ["Unexpected token: '", danglingToken[0], "'. You might have too many close parentheses in this file."].join(''));
		}
		i += 1;
	}
	let resolverCtx = Resolver_new(compiler[0], rootEntities, compiler[9]);
	Resolve(resolverCtx);
	let m = CompiledModule_new(moduleId);
	m[1] = sourceCode;
	CompiledModule_AddLambdas(m, resolverCtx[6]);
	CompiledModule_InitializeLookups(m, resolverCtx[1], resolverCtx[3]);
	i = 0;
	while (i < files.length) {
		let file = files[i];
		file[8] = m;
		i += 1;
	}
	return m;
};

let CompilerContext_new = function(rootId, flavorId, extensionVersionId, extensionNames) {
	let ctx = [StaticContext_new(), rootId, {}, {}, null, {}, null, extensionVersionId, flavorId, []];
	let i = 0;
	while (i < extensionNames.length) {
		ctx[9].push(extensionNames[i]);
		i += 1;
	}
	ctx[5][rootId] = true;
	let builtinFiles = {};
	builtinFiles["builtins.script"] = GetSourceForBuiltinModule("builtins");
	PUBLIC_SupplyFilesForModule(ctx, "{BUILTIN}", builtinFiles, true, true);
	return ctx;
};

let ConstEntity_new = function(constToken, nameToken, constValue) {
	let c = [constValue, null];
	c[1] = AbstractEntity_new(constToken, 2, c);
	c[1][4] = nameToken;
	c[1][3] = nameToken[0];
	return c;
};

let convertToBuffer = function(flatRows) {
	let buf = null;
	let length = flatRows.length;
	let i = 0;
	while (i < length) {
		buf = join2(buf, ByteCodeBuffer_fromRow(flatRows[i]));
		i += 1;
	}
	return buf;
};

let create0 = function(opCode, token, stringArg) {
	return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, PST$createNewArray(0)));
};

let create1 = function(opCode, token, stringArg, arg1) {
	let args = PST$createNewArray(1);
	args[0] = arg1;
	return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
};

let create2 = function(opCode, token, stringArg, arg1, arg2) {
	let args = PST$createNewArray(2);
	args[0] = arg1;
	args[1] = arg2;
	return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
};

let create3 = function(opCode, token, stringArg, arg1, arg2, arg3) {
	let args = PST$createNewArray(3);
	args[0] = arg1;
	args[1] = arg2;
	args[2] = arg3;
	return ByteCodeBuffer_fromRow(ByteCodeRow_new(opCode, token, stringArg, args));
};

let createFakeToken = function(tokens, type, value, line, col) {
	return Token_new(value, type, tokens[3], line, col);
};

let createFakeTokenFromTemplate = function(template, value, tokenType) {
	return Token_new(value, tokenType, template[1], template[3], template[4]);
};

let DotField_getVariableRootedDottedChain = function(outermostDotField, errorMessage) {
	let chain = [];
	chain.push(outermostDotField[7]);
	let walker = outermostDotField[2];
	while (walker != null) {
		chain.push(walker[7]);
		if (walker[1] == 11) {
			walker = walker[2];
		} else if (walker[1] == 31) {
			walker = null;
		} else if (errorMessage != null) {
			Errors_Throw(walker[0], errorMessage);
		} else {
			return null;
		}
	}
	chain.reverse();
	return [...(chain)];
};

let Entity_getMemberLookup = function(staticCtx, entity) {
	if (entity[1] == 1) {
		return entity[2][5];
	}
	if (entity[1] == 7) {
		return entity[2][0];
	}
	if (entity[1] == 9) {
		return entity[2][0];
	}
	return staticCtx[1];
};

let EntityParser_ClassifyToken = function(t) {
	switch (t.charAt(0)) {
		case "c":
			if (t == "const") {
				return 3;
			}
			if (t == "constructor") {
				return 6;
			}
			if (t == "class") {
				return 5;
			}
			break;
		case "e":
			if (t == "enum") {
				return 4;
			}
			break;
		case "f":
			if (t == "field") {
				return 7;
			}
			if (t == "function") {
				return 1;
			}
			break;
		case "i":
			if (t == "import") {
				return 9;
			}
			break;
		case "n":
			if (t == "namespace") {
				return 2;
			}
			break;
		case "p":
			if (t == "property") {
				return 8;
			}
			break;
		case "}":
			if (t == "}") {
				return 10;
			}
			break;
	}
	return 0;
};

let EntityResolver_ConvertFieldDefaultValueIntoSetter = function(fld) {
	if (fld[1] == null) {
		fail("");
	}
	let root = null;
	if (fld[2][7]) {
		root = Expression_createClassReference(null, fld[2][8]);
	} else {
		root = Expression_createThisReference(null);
	}
	let target = Expression_createDotField(root, null, fld[2][3]);
	let equal = fld[1];
	return Statement_createAssignment(target, equal, fld[0]);
};

let EntityResolver_DetermineMemberOffsets = function(classDef) {
	let i = 0;
	if (classDef[2] != null) {
		return;
	}
	let parent = classDef[1];
	if (parent != null) {
		EntityResolver_DetermineMemberOffsets(parent);
	}
	classDef[2] = {};
	classDef[3] = {};
	let newDirectMembers = [];
	let staticFieldNames = [];
	let staticMethodNames = [];
	if (parent != null) {
		let parentKeys = Object.keys(parent[3]);
		i = 0;
		while (i < parentKeys.length) {
			let parentKey = parentKeys[i];
			classDef[3][parentKey] = parent[3][parentKey];
			i += 1;
		}
	}
	let nextOffset = Object.keys(classDef[3]).length;
	let memberNames = PST$sortedCopyOfArray(Object.keys(classDef[5]));
	i = 0;
	while (i < memberNames.length) {
		let memberName = memberNames[i];
		let member = classDef[5][memberName];
		if (!member[7] && (member[1] == 5 || member[1] == 6)) {
			let offset = 0;
			if (!(classDef[3][memberName] !== undefined)) {
				offset = nextOffset;
				nextOffset += 1;
				newDirectMembers.push(memberName);
				classDef[2][memberName] = offset;
				classDef[3][memberName] = offset;
			}
		}
		i += 1;
	}
	classDef[4] = [...(newDirectMembers)];
};

let EntityResolver_GetNextAutoVarId = function(resolver) {
	let id = resolver[10];
	resolver[10] += 1;
	return id;
};

let EntityResolver_ResetAutoVarId = function(resolver) {
	resolver[10] = 0;
};

let EntityResolver_ResolveFunctionFirstPass = function(resolver, funcDef) {
	let i = 0;
	funcDef[5] = {};
	resolver[7] = funcDef[3];
	resolver[9] = null;
	i = 0;
	while (i < funcDef[0].length) {
		let arg = funcDef[0][i];
		if (funcDef[5][arg[0]] !== undefined) {
			Errors_Throw(arg, ["There are multiple arguments named '", arg[0], "'."].join(''));
		}
		funcDef[5][arg[0]] = true;
		let defVal = funcDef[1][i];
		if (defVal != null) {
			funcDef[1][i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, defVal);
		}
		i += 1;
	}
	let preBaseFieldInit = [];
	let postBaseFieldInit = [];
	let baseCtorInvocation = [];
	let isCtor = funcDef[3][1] == 3;
	let ctorEnt = null;
	if (isCtor) {
		ctorEnt = funcDef;
		let siblings = Entity_getMemberLookup(resolver[0], funcDef[3][8]);
		let fields = [];
		let fieldKeys = PST$sortedCopyOfArray(Object.keys(siblings));
		i = 0;
		while (i < fieldKeys.length) {
			let sibling = siblings[fieldKeys[i]];
			if (sibling[1] == 5 && sibling[7] == ctorEnt[3][7]) {
				let fe = sibling[2];
				if (fe[0] != null) {
					fields.push(fe);
				}
			}
			i += 1;
		}
		i = 0;
		while (i < fields.length) {
			let fld = fields[i];
			fld[0] = ExpressionResolver_ResolveExpressionFirstPass(resolver, fld[0]);
			let setter = EntityResolver_ConvertFieldDefaultValueIntoSetter(fld);
			if (IsExpressionConstant(fld[0])) {
				preBaseFieldInit.push(setter);
			} else {
				postBaseFieldInit.push(setter);
			}
			i += 1;
		}
	}
	if (isCtor && funcDef[3][8][1] == 1 && funcDef[3][8][2][1] != null) {
		let baseCtor = funcDef[3][0];
		let baseCtorParen = funcDef[3][0];
		let baseCtorRef = Expression_createBaseCtorReference(baseCtor);
		let baseCtorInvoke = Expression_createFunctionInvocation(baseCtorRef, baseCtorParen, ctorEnt[2]);
		let baseCtorStmnt = Statement_createExpressionAsStatement(baseCtorInvoke);
		baseCtorStmnt = StatementResolver_ResolveStatementFirstPass(resolver, baseCtorStmnt);
		baseCtorInvocation.push(baseCtorStmnt);
	}
	StatementResolver_ResolveStatementArrayFirstPass(resolver, funcDef[4]);
	let flattened = [];
	i = 0;
	while (i < preBaseFieldInit.length) {
		flattened.push(preBaseFieldInit[i]);
		i += 1;
	}
	i = 0;
	while (i < baseCtorInvocation.length) {
		flattened.push(baseCtorInvocation[i]);
		i += 1;
	}
	i = 0;
	while (i < postBaseFieldInit.length) {
		flattened.push(postBaseFieldInit[i]);
		i += 1;
	}
	i = 0;
	while (i < funcDef[4].length) {
		flattened.push(funcDef[4][i]);
		i += 1;
	}
	let lastStatement = null;
	if (flattened.length > 0) {
		lastStatement = flattened[flattened.length - 1];
	}
	let autoReturnNeeded = lastStatement == null || (lastStatement[1] != 9 && lastStatement[1] != 11);
	if (autoReturnNeeded) {
		flattened.push(Statement_createReturn(null, Expression_createNullConstant(null)));
	}
	funcDef[4] = [...(flattened)];
	resolver[7] = null;
	resolver[9] = null;
};

let EntityResolver_ResolveFunctionSecondPass = function(resolver, funcDef) {
	let i = 0;
	resolver[7] = funcDef[3];
	resolver[9] = null;
	i = 0;
	while (i < funcDef[1].length) {
		let defVal = funcDef[1][i];
		if (defVal != null) {
			funcDef[1][i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, defVal);
		}
		i += 1;
	}
	StatementResolver_ResolveStatementArraySecondPass(resolver, funcDef[4]);
	if (funcDef[4].length == 0 || funcDef[4][funcDef[4].length - 1][1] != 9) {
		let newCode = [];
		i = 0;
		while (i < funcDef[4].length) {
			newCode.push(funcDef[4][i]);
			i += 1;
		}
		newCode.push(Statement_createReturn(null, Expression_createNullConstant(null)));
		funcDef[4] = [...(newCode)];
	}
	resolver[7] = null;
	resolver[9] = null;
};

let EnumEntity_new = function(enumToken, nameToken, memberNames, memberValues) {
	let e = [memberNames, memberValues, null];
	e[2] = AbstractEntity_new(enumToken, 4, e);
	e[2][4] = nameToken;
	e[2][3] = nameToken[0];
	if (memberNames.length == 0) {
		Errors_Throw(enumToken, "This enum definition is empty.");
	}
	let collisionCheck = {};
	let isImplicit = memberValues[0] == null;
	let i = 0;
	while (i < memberNames.length) {
		let name = memberNames[i];
		if (collisionCheck[name[0]] !== undefined) {
			Errors_Throw(name, "This enum value name collides with a previous definition.");
		}
		let valueIsImplicit = memberValues[i] == null;
		if (valueIsImplicit != isImplicit) {
			Errors_Throw(enumToken, "This enum definition defines values for some but not all members. Mixed implicit/explicit definitions are not allowed.");
		}
		if (isImplicit) {
			e[1][i] = Expression_createIntegerConstant(null, i + 1);
		}
		i += 1;
	}
	return e;
};

let Errors_Throw = function(token, msg) {
	_Errors_ThrowImpl(1, token, msg, "");
};

let Errors_ThrowEof = function(fileName, msg) {
	_Errors_ThrowImpl(2, null, fileName, msg);
};

let Errors_ThrowGeneralError = function(msg) {
	_Errors_ThrowImpl(3, null, msg, "");
};

let Errors_ThrowNotImplemented = function(token, optionalMsg) {
	if (optionalMsg == null) {
		optionalMsg = "";
	}
	Errors_Throw(token, "***NOT IMPLEMENTED*** " + optionalMsg.trim());
};

let ExportUtil_exportBundle = function(flavorId, extVersionId, bundle) {
	let i = 0;
	let j = 0;
	let flavor = bsbFromLenString(flavorId);
	let version = bsbFromLenString(extVersionId);
	let commonScriptMajor = 0;
	let commonScriptMinor = 1;
	let commonScriptPatch = 0;
	let header = bsbJoin4(bsbFromUtf8String("PXCS"), bsbFrom4Bytes(0, commonScriptMajor, commonScriptMinor, commonScriptPatch), flavor, version);
	let metadata = bsbJoin3(bsbFromUtf8String("MTD"), bsbFromInt(bundle[7]), bsbFromInt(bundle[8]));
	let tokenData = bsbJoin2(bsbFromUtf8String("TOK"), bsbFromInt(bundle[6].length - 1));
	let fileNameToOffset = {};
	let tokenFileNames = null;
	i = 1;
	while (i < bundle[6].length) {
		let tok = bundle[6][i];
		let filename = tok[1];
		if (!(fileNameToOffset[filename] !== undefined)) {
			fileNameToOffset[filename] = Object.keys(fileNameToOffset).length;
			tokenFileNames = bsbJoin2(tokenFileNames, bsbFromLenString(filename));
		}
		i += 1;
	}
	tokenData = bsbJoin3(tokenData, bsbFromInt(Object.keys(fileNameToOffset).length), tokenFileNames);
	i = 1;
	while (i < bundle[6].length) {
		let tok = bundle[6][i];
		let filename = tok[1];
		let fileOffset = fileNameToOffset[filename];
		tokenData = bsbJoin4(tokenData, bsbFromInt(fileOffset), bsbFromInt(tok[3]), bsbFromInt(tok[4]));
		i += 1;
	}
	let stringData = bsbJoin2(bsbFromUtf8String("STR"), bsbFromInt(bundle[0].length - 1));
	i = 1;
	while (i < bundle[0].length) {
		let val = bundle[0][i];
		stringData = bsbJoin2(stringData, bsbFromLenString(val));
		i += 1;
	}
	let entityAcc = null;
	i = 1;
	while (i < bundle[2].length) {
		let fn = bundle[2][i];
		entityAcc = bsbJoin2(entityAcc, bsbJoin5(bsbFromInt(fn[1]), bsbFromInt(fn[2]), bsbFromLenString(fn[3]), bsbFromInt(fn[0].length), ExportUtil_exportCode(fn[0])));
		i += 1;
	}
	i = 1;
	while (i < bundle[3].length) {
		let fn = bundle[3][i];
		entityAcc = bsbJoin2(entityAcc, bsbJoin4(bsbFromInt(fn[1]), bsbFromInt(fn[2]), bsbFromInt(fn[0].length), ExportUtil_exportCode(fn[0])));
		i += 1;
	}
	i = 1;
	while (i < bundle[5].length) {
		let bei = bundle[5][i];
		let memberCount = bei[1].length;
		entityAcc = bsbJoin2(entityAcc, bsbFromInt(memberCount));
		j = 0;
		while (j < memberCount) {
			entityAcc = bsbJoin3(entityAcc, bsbFromLenString(bei[1][j]), bsbFromInt(bei[2][j]));
			j++;
		}
		i += 1;
	}
	i = 1;
	while (i < bundle[4].length) {
		let bci = bundle[4][i];
		let classInfo = bsbJoin8(bsbFromLenString(bci[2]), bsbFromInt(bci[1]), bsbFromInt(bci[3]), bsbFromInt(bci[4]), bsbFromInt(bci[6].length), bsbFromInt(Object.keys(bci[5]).length), bsbFromInt(bci[8].length), bsbFromInt(bci[7].length));
		j = 0;
		while (j < bci[6].length) {
			let memberName = bci[6][j];
			let isMethod = bci[5][memberName] !== undefined;
			let info = 0;
			if (isMethod) {
				info = 1;
			}
			classInfo = bsbJoin3(classInfo, bsbFromLenString(memberName), bsbFromInt(info));
			j += 1;
		}
		let methodInfo = null;
		let methodNames = PST$sortedCopyOfArray(Object.keys(bci[5]));
		j = 0;
		while (j < methodNames.length) {
			let methodName = methodNames[j];
			methodInfo = bsbJoin3(methodInfo, bsbFromLenString(methodName), bsbFromInt(bci[5][methodName]));
			j += 1;
		}
		let staticFields = null;
		let fieldNames = PST$sortedCopyOfArray([...(bci[8])]);
		j = 0;
		while (j < fieldNames.length) {
			let staticField = fieldNames[j];
			staticFields = bsbJoin2(staticFields, bsbFromLenString(staticField));
			j += 1;
		}
		let staticMethods = null;
		let staticMethodNames = PST$sortedCopyOfArray([...(bci[7])]);
		j = 0;
		while (j < staticMethodNames.length) {
			let staticMethod = staticMethodNames[j];
			let funcId = bci[5][staticMethod];
			staticMethods = bsbJoin3(staticMethods, bsbFromLenString(staticMethod), bsbFromInt(funcId));
			j += 1;
		}
		entityAcc = bsbJoin5(entityAcc, classInfo, methodInfo, staticFields, staticMethods);
		i += 1;
	}
	let entityHeader = bsbJoin5(bsbFromUtf8String("ENT"), bsbFromInt(bundle[2].length - 1), bsbFromInt(bundle[5].length - 1), bsbFromInt(bundle[4].length - 1), bsbFromInt(bundle[3].length - 1));
	let entityData = bsbJoin2(entityHeader, entityAcc);
	let full = bsbJoin5(header, metadata, stringData, tokenData, entityData);
	return bsbFlatten(full);
};

let ExportUtil_exportCode = function(rows) {
	let bsb = null;
	let i = 0;
	while (i < rows.length) {
		let row = rows[i];
		let args = row[3];
		let argsLen = args.length;
		let flags = argsLen * 4;
		let bsbStringArg = null;
		let bsbToken = null;
		if (row[1] != null) {
			flags += 1;
			bsbStringArg = bsbFromInt(row[4]);
		}
		if (row[2] != null) {
			flags += 2;
			bsbToken = bsbFromInt(row[5]);
		}
		let bsbRow = bsbJoin4(bsbFromInt(row[0]), bsbFromInt(flags), bsbStringArg, bsbToken);
		let j = 0;
		while (j < argsLen) {
			bsbRow = bsbJoin2(bsbRow, bsbFromInt(args[j]));
			j += 1;
		}
		bsb = bsbJoin2(bsb, bsbRow);
		i += 1;
	}
	return bsb;
};

let Expression_cloneWithNewToken = function(token, expr) {
	return [token, expr[1], expr[2], expr[3], expr[4], expr[5], expr[6], expr[7], expr[8], expr[9], expr[10], expr[11], expr[12], expr[13], expr[14], expr[15], expr[16]];
};

let Expression_createBaseCtorReference = function(token) {
	return Expression_new(token, 2);
};

let Expression_createBaseReference = function(token) {
	return Expression_new(token, 1);
};

let Expression_createBinaryOp = function(left, op, right) {
	let pair = Expression_new(left[0], 3);
	pair[5] = op;
	pair[3] = left;
	pair[4] = right;
	return pair;
};

let Expression_createBoolConstant = function(token, val) {
	let expr = Expression_new(token, 5);
	expr[6] = val;
	return expr;
};

let Expression_createBracketIndex = function(root, bracketToken, index) {
	let bracketIndex = Expression_new(root[0], 20);
	bracketIndex[2] = root;
	bracketIndex[5] = bracketToken;
	bracketIndex[4] = index;
	return bracketIndex;
};

let Expression_createClassReference = function(firstToken, classDef) {
	let classRef = Expression_new(firstToken, 7);
	classRef[10] = classDef;
	classRef[6] = false;
	return classRef;
};

let Expression_createConstructorInvocation = function(firstToken, classDef, invokeToken, args) {
	let ctorInvoke = Expression_new(firstToken, 8);
	ctorInvoke[10] = classDef;
	ctorInvoke[12] = args;
	ctorInvoke[5] = invokeToken;
	return ctorInvoke;
};

let Expression_createConstructorReference = function(newToken, nameChain) {
	let ctor = Expression_new(newToken, 9);
	ctor[2] = nameChain;
	return ctor;
};

let Expression_createDictionaryDefinition = function(openDict, keys, values) {
	let expr = Expression_new(openDict, 10);
	expr[13] = keys;
	expr[14] = values;
	let i = 0;
	while (i < keys.length) {
		let key = keys[i];
		if (key[1] != 28 && key[1] != 22) {
			Errors_Throw(key[0], "Only string and integer constants can be used as dictionary keys");
		}
		i += 1;
	}
	return expr;
};

let Expression_createDotField = function(root, dotToken, name) {
	let df = Expression_new(root[0], 11);
	df[2] = root;
	df[5] = dotToken;
	df[7] = name;
	return df;
};

let Expression_createEnumConstant = function(firstToken, enumDef, name, value) {
	let enumConst = Expression_new(firstToken, 12);
	enumConst[10] = enumDef;
	enumConst[7] = name;
	enumConst[8] = value;
	return enumConst;
};

let Expression_createEnumReference = function(firstToken, enumDef) {
	let enumRef = Expression_new(firstToken, 13);
	enumRef[10] = enumDef;
	return enumRef;
};

let Expression_createExtensionInvocation = function(firstToken, name, args) {
	let extInvoke = Expression_new(firstToken, 14);
	extInvoke[7] = name;
	extInvoke[12] = args;
	return extInvoke;
};

let Expression_createExtensionReference = function(prefixToken, name) {
	let extRef = Expression_new(prefixToken, 15);
	extRef[7] = name;
	return extRef;
};

let Expression_createFloatConstant = function(token, val) {
	let expr = Expression_new(token, 16);
	expr[9] = val;
	return expr;
};

let Expression_createFunctionInvocation = function(root, parenToken, args) {
	let funcInvoke = Expression_new(root[0], 17);
	funcInvoke[2] = root;
	funcInvoke[5] = parenToken;
	funcInvoke[12] = args;
	return funcInvoke;
};

let Expression_createFunctionReference = function(firstToken, name, funcDef) {
	let funcRef = Expression_new(firstToken, 18);
	funcRef[7] = name;
	funcRef[10] = funcDef;
	return funcRef;
};

let Expression_createImportReference = function(firstToken, importStatement) {
	let impRef = Expression_new(firstToken, 19);
	impRef[11] = importStatement;
	return impRef;
};

let Expression_createInlineIncrement = function(firstToken, root, incrementOp, isPrefix) {
	let expr = Expression_new(firstToken, 21);
	expr[5] = incrementOp;
	expr[2] = root;
	expr[6] = isPrefix;
	switch (root[1]) {
		case 11:
			break;
		case 20:
			break;
		case 31:
			break;
		default:
			Errors_Throw(incrementOp, ["The '", incrementOp[0], "' operator is not allowed on this type of expression."].join(''));
			break;
	}
	return expr;
};

let Expression_createIntegerConstant = function(token, val) {
	let expr = Expression_new(token, 22);
	expr[8] = val;
	return expr;
};

let Expression_createLambda = function(firstToken, argNameTokens, argDefaultValues, arrowToken, code) {
	let expr = Expression_new(firstToken, 23);
	expr[15] = argNameTokens;
	expr[14] = argDefaultValues;
	expr[5] = arrowToken;
	expr[16] = code;
	return expr;
};

let Expression_createListDefinition = function(openList, items) {
	let expr = Expression_new(openList, 24);
	expr[14] = items;
	return expr;
};

let Expression_createNamespaceReference = function(firstToken, nsDef) {
	let nsRef = Expression_new(firstToken, 32);
	nsRef[10] = nsDef;
	return nsRef;
};

let Expression_createNegatePrefix = function(opToken, root) {
	let t = 25;
	if (opToken[0] == "!") {
		t = 6;
	}
	if (opToken[0] == "~") {
		t = 4;
	}
	let expr = Expression_new(opToken, t);
	expr[5] = opToken;
	expr[2] = root;
	return expr;
};

let Expression_createNullConstant = function(token) {
	return Expression_new(token, 26);
};

let Expression_createSliceExpression = function(rootExpression, bracketToken, start, end, step) {
	let sliceExpr = Expression_new(rootExpression[0], 27);
	sliceExpr[2] = rootExpression;
	sliceExpr[5] = bracketToken;
	let args = PST$createNewArray(3);
	args[0] = start;
	args[1] = end;
	args[2] = step;
	sliceExpr[12] = args;
	return sliceExpr;
};

let Expression_createStringConstant = function(token, val) {
	let expr = Expression_new(token, 28);
	expr[7] = val;
	return expr;
};

let Expression_createTernary = function(condition, op, trueValue, falseValue) {
	let ternary = Expression_new(condition[0], 29);
	ternary[2] = condition;
	ternary[5] = op;
	ternary[3] = trueValue;
	ternary[4] = falseValue;
	return ternary;
};

let Expression_createThisReference = function(token) {
	return Expression_new(token, 30);
};

let Expression_createTypeof = function(typeofToken, root) {
	let typeofExpr = Expression_new(typeofToken, 33);
	typeofExpr[2] = root;
	return typeofExpr;
};

let Expression_createVariable = function(token, varName) {
	let expr = Expression_new(token, 31);
	expr[7] = varName;
	return expr;
};

let Expression_new = function(firstToken, type) {
	return [firstToken, type, null, null, null, null, false, null, 0, 0.0, null, null, null, null, null, null, null];
};

let ExpressionResolver_FindLocallyReferencedEntity = function(staticCtx, lookup, name) {
	if (lookup[name] !== undefined) {
		return lookup[name];
	}
	if (lookup[".."] !== undefined) {
		let prevLevelLookup = Entity_getMemberLookup(staticCtx, lookup[".."]);
		return ExpressionResolver_FindLocallyReferencedEntity(staticCtx, prevLevelLookup, name);
	}
	return null;
};

let ExpressionResolver_FirstPass_BaseCtorReference = function(resolver, baseCtor) {
	return baseCtor;
};

let ExpressionResolver_FirstPass_BinaryOp = function(resolver, binOp) {
	binOp[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, binOp[3]);
	binOp[4] = ExpressionResolver_ResolveExpressionFirstPass(resolver, binOp[4]);
	let isBitwise = false;
	let token = binOp[5][0];
	switch (token.charAt(0)) {
		case "|":
			isBitwise = token == "|";
			break;
		case "&":
			isBitwise = token == "&";
			break;
		case "^":
			isBitwise = token == "^";
			break;
		case "<":
			isBitwise = token == "<<";
			break;
		case ">":
			isBitwise = token == ">>" || token == ">>>";
			break;
	}
	if (isBitwise) {
		binOp[3] = ExpressionResolver_IntegerRequired(resolver, binOp[3]);
		binOp[4] = ExpressionResolver_IntegerRequired(resolver, binOp[4]);
	}
	return binOp;
};

let ExpressionResolver_FirstPass_BitwiseNot = function(resolver, bwn) {
	bwn[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, bwn[2]);
	bwn[2] = ExpressionResolver_IntegerRequired(resolver, bwn[2]);
	return bwn;
};

let ExpressionResolver_FirstPass_BoolConst = function(resolver, bc) {
	return bc;
};

let ExpressionResolver_FirstPass_BoolNot = function(resolver, booNot) {
	booNot[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, booNot[2]);
	return booNot;
};

let ExpressionResolver_FirstPass_ConstructorInvocation = function(resolver, ctorInvoke) {
	fail("not implemented");
	return null;
};

let ExpressionResolver_FirstPass_ConstructorReference = function(resolver, ctorRef) {
	ctorRef[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, ctorRef[2]);
	return ctorRef;
};

let ExpressionResolver_FirstPass_DictionaryDefinition = function(resolver, dictDef) {
	let length = dictDef[13].length;
	let i = 0;
	while (i < length) {
		dictDef[13][i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, dictDef[13][i]);
		dictDef[14][i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, dictDef[14][i]);
		i += 1;
	}
	return dictDef;
};

let ExpressionResolver_FirstPass_DotField = function(resolver, dotField) {
	dotField[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, dotField[2]);
	let fieldName = dotField[7];
	switch (dotField[2][1]) {
		case 19:
			let importRef = dotField[2][11];
			let moduleRef = importRef[5];
			let output = LookupUtil_tryCreateModuleMemberReference(moduleRef, dotField[0], fieldName);
			if (output == null) {
				Errors_Throw(dotField[5], ["The module does not have a member named '", fieldName, "'"].join(''));
			}
			return output;
		case 13:
			let enumRef = dotField[2][10][2];
			let i = 0;
			while (i < enumRef[0].length) {
				if (enumRef[0][i][0] == fieldName) {
					return Expression_createEnumConstant(dotField[0], enumRef[2], fieldName, enumRef[1][i][8]);
				}
				i += 1;
			}
			Errors_Throw(dotField[5], ["The enum ", enumRef[2][5], " does not have a member named '", fieldName, "'"].join(''));
			break;
		case 32:
			let nsEntity = dotField[2][10][2];
			if (!(nsEntity[0][fieldName] !== undefined)) {
				Errors_Throw(dotField[5], ["There is no member of this namespace named '", fieldName, "'."].join(''));
			}
			let referencedEntity = nsEntity[0][fieldName];
			return ExpressionResolver_WrapEntityIntoReferenceExpression(resolver, dotField[0], referencedEntity);
	}
	return dotField;
};

let ExpressionResolver_FirstPass_FloatConstant = function(resolver, floatConst) {
	return floatConst;
};

let ExpressionResolver_FirstPass_FunctionInvocation = function(resolver, funcInvoke) {
	if (funcInvoke[2][1] == 15) {
		ExpressionResolver_ResolveExpressionArrayFirstPass(resolver, funcInvoke[12]);
		return Expression_createExtensionInvocation(funcInvoke[0], funcInvoke[2][7], funcInvoke[12]);
	}
	funcInvoke[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, funcInvoke[2]);
	ExpressionResolver_ResolveExpressionArrayFirstPass(resolver, funcInvoke[12]);
	return funcInvoke;
};

let ExpressionResolver_FirstPass_Index = function(resolver, indexExpr) {
	indexExpr[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, indexExpr[2]);
	indexExpr[4] = ExpressionResolver_ResolveExpressionFirstPass(resolver, indexExpr[4]);
	return indexExpr;
};

let ExpressionResolver_FirstPass_InlineIncrement = function(resolver, inlineIncr) {
	inlineIncr[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, inlineIncr[2]);
	return inlineIncr;
};

let ExpressionResolver_FirstPass_IntegerConstant = function(resolver, intConst) {
	return intConst;
};

let ExpressionResolver_FirstPass_Lambda = function(resolver, lamb) {
	let i = 0;
	let argNames = [];
	i = 0;
	while (i < lamb[15].length) {
		argNames.push(lamb[15][i]);
		i += 1;
	}
	let argValues = [];
	i = 0;
	while (i < lamb[14].length) {
		argValues.push(lamb[14][i]);
		i += 1;
	}
	let code = [];
	i = 0;
	while (i < lamb[16].length) {
		code.push(lamb[16][i]);
		i += 1;
	}
	let lambdaEnt = FunctionEntity_BuildLambda(resolver[7][9], lamb[0], argNames, argValues, code);
	Resolver_ReportNewLambda(resolver, lambdaEnt);
	lamb[10] = lambdaEnt[3];
	return lamb;
};

let ExpressionResolver_FirstPass_ListDefinition = function(resolver, listDef) {
	let i = 0;
	while (i < listDef[14].length) {
		listDef[14][i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, listDef[14][i]);
		i += 1;
	}
	return listDef;
};

let ExpressionResolver_FirstPass_NegativeSign = function(resolver, negSign) {
	negSign[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, negSign[2]);
	return negSign;
};

let ExpressionResolver_FirstPass_NullConst = function(resolver, nullConst) {
	return nullConst;
};

let ExpressionResolver_FirstPass_Slice = function(resolver, slice) {
	slice[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, slice[2]);
	let i = 0;
	while (i < 3) {
		if (slice[12][i] != null) {
			slice[12][i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, slice[12][i]);
		}
		i += 1;
	}
	return slice;
};

let ExpressionResolver_FirstPass_StringConstant = function(resolver, strConst) {
	return strConst;
};

let ExpressionResolver_FirstPass_Ternary = function(resolver, ternary) {
	ternary[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary[2]);
	ternary[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary[3]);
	ternary[4] = ExpressionResolver_ResolveExpressionFirstPass(resolver, ternary[4]);
	return ternary;
};

let ExpressionResolver_FirstPass_This = function(resolver, thisExpr) {
	return thisExpr;
};

let ExpressionResolver_FirstPass_TypeOf = function(resolver, typeofExpr) {
	typeofExpr[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, typeofExpr[2]);
	return typeofExpr;
};

let ExpressionResolver_FirstPass_Variable = function(resolver, varExpr) {
	let name = varExpr[7];
	let localEntity = ExpressionResolver_FindLocallyReferencedEntity(resolver[0], resolver[1], name);
	if (localEntity != null) {
		return ExpressionResolver_WrapEntityIntoReferenceExpression(resolver, varExpr[0], localEntity);
	}
	let importedRef = LookupUtil_DoFirstPassVariableLookupThroughImports(resolver, varExpr[0], name);
	if (importedRef != null) {
		return importedRef;
	}
	return varExpr;
};

let ExpressionResolver_IntegerRequired = function(resolver, expr) {
	switch (expr[1]) {
		case 12:
			let enumParent = expr[10][2];
			let enumMem = expr[7];
			let i = 0;
			while (i < enumParent[1].length) {
				if (enumMem == enumParent[0][i][0]) {
					let val = enumParent[1][i];
					if (val[1] != 22) {
						fail("");
					}
					return val;
				}
				i += 1;
			}
			Errors_Throw(expr[0], ["The enum '", enumParent[2][5], "' does not have a member named '", enumMem, "'."].join(''));
			break;
		case 5:
			Errors_Throw(expr[0], "An integer is expected here.");
			break;
		case 16:
			Errors_Throw(expr[0], "An integer is expected here.");
			break;
		case 28:
			Errors_Throw(expr[0], "An integer is expected here.");
			break;
		case 6:
			Errors_Throw(expr[0], "An integer is expected here.");
			break;
	}
	return expr;
};

let ExpressionResolver_ResolveExpressionArrayFirstPass = function(resolver, arr) {
	let i = 0;
	while (i < arr.length) {
		arr[i] = ExpressionResolver_ResolveExpressionFirstPass(resolver, arr[i]);
		i += 1;
	}
};

let ExpressionResolver_ResolveExpressionArraySecondPass = function(resolver, arr) {
	let i = 0;
	while (i < arr.length) {
		arr[i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, arr[i]);
		i += 1;
	}
};

let ExpressionResolver_ResolveExpressionFirstPass = function(resolver, expr) {
	switch (expr[1]) {
		case 2:
			return ExpressionResolver_FirstPass_BaseCtorReference(resolver, expr);
		case 3:
			return ExpressionResolver_FirstPass_BinaryOp(resolver, expr);
		case 4:
			return ExpressionResolver_FirstPass_BitwiseNot(resolver, expr);
		case 5:
			return ExpressionResolver_FirstPass_BoolConst(resolver, expr);
		case 6:
			return ExpressionResolver_FirstPass_BoolNot(resolver, expr);
		case 8:
			return ExpressionResolver_FirstPass_ConstructorInvocation(resolver, expr);
		case 9:
			return ExpressionResolver_FirstPass_ConstructorReference(resolver, expr);
		case 10:
			return ExpressionResolver_FirstPass_DictionaryDefinition(resolver, expr);
		case 11:
			return ExpressionResolver_FirstPass_DotField(resolver, expr);
		case 16:
			return ExpressionResolver_FirstPass_FloatConstant(resolver, expr);
		case 17:
			return ExpressionResolver_FirstPass_FunctionInvocation(resolver, expr);
		case 20:
			return ExpressionResolver_FirstPass_Index(resolver, expr);
		case 21:
			return ExpressionResolver_FirstPass_InlineIncrement(resolver, expr);
		case 22:
			return ExpressionResolver_FirstPass_IntegerConstant(resolver, expr);
		case 23:
			return ExpressionResolver_FirstPass_Lambda(resolver, expr);
		case 24:
			return ExpressionResolver_FirstPass_ListDefinition(resolver, expr);
		case 25:
			return ExpressionResolver_FirstPass_NegativeSign(resolver, expr);
		case 26:
			return ExpressionResolver_FirstPass_NullConst(resolver, expr);
		case 27:
			return ExpressionResolver_FirstPass_Slice(resolver, expr);
		case 28:
			return ExpressionResolver_FirstPass_StringConstant(resolver, expr);
		case 29:
			return ExpressionResolver_FirstPass_Ternary(resolver, expr);
		case 30:
			return ExpressionResolver_FirstPass_This(resolver, expr);
		case 33:
			return ExpressionResolver_FirstPass_TypeOf(resolver, expr);
		case 31:
			return ExpressionResolver_FirstPass_Variable(resolver, expr);
		case 15:
			Errors_Throw(expr[0], "Extension method references must be invoked immediately.");
			break;
		default:
			Errors_ThrowNotImplemented(expr[0], "first pass for this type");
			break;
	}
	return expr;
};

let ExpressionResolver_ResolveExpressionSecondPass = function(resolver, expr) {
	switch (expr[1]) {
		case 2:
			return ExpressionResolver_SecondPass_BaseCtorReference(resolver, expr);
		case 3:
			return ExpressionResolver_SecondPass_BinaryOp(resolver, expr);
		case 4:
			return ExpressionResolver_SecondPass_BitwiseNot(resolver, expr);
		case 5:
			return ExpressionResolver_SecondPass_BoolConst(resolver, expr);
		case 6:
			return ExpressionResolver_SecondPass_BoolNot(resolver, expr);
		case 7:
			return ExpressionResolver_SecondPass_ClassReference(resolver, expr);
		case 9:
			return ExpressionResolver_SecondPass_ConstructorReference(resolver, expr, false);
		case 10:
			return ExpressionResolver_SecondPass_DictionaryDefinition(resolver, expr);
		case 11:
			return ExpressionResolver_SecondPass_DotField(resolver, expr);
		case 12:
			return ExpressionResolver_SecondPass_EnumConstant(resolver, expr);
		case 14:
			return ExpressionResolver_SecondPass_ExtensionInvocation(resolver, expr);
		case 16:
			return ExpressionResolver_SecondPass_FloatConstant(resolver, expr);
		case 17:
			return ExpressionResolver_SecondPass_FunctionInvocation(resolver, expr);
		case 18:
			return ExpressionResolver_SecondPass_FunctionReference(resolver, expr);
		case 19:
			return ExpressionResolver_SecondPass_ImportReference(resolver, expr);
		case 20:
			return ExpressionResolver_SecondPass_Index(resolver, expr);
		case 21:
			return ExpressionResolver_SecondPass_InlineIncrement(resolver, expr);
		case 22:
			return ExpressionResolver_SecondPass_IntegerConstant(resolver, expr);
		case 23:
			return ExpressionResolver_SecondPass_Lambda(resolver, expr);
		case 24:
			return ExpressionResolver_SecondPass_ListDefinition(resolver, expr);
		case 32:
			return ExpressionResolver_SecondPass_NamespaceReference(resolver, expr);
		case 25:
			return ExpressionResolver_SecondPass_NegativeSign(resolver, expr);
		case 26:
			return ExpressionResolver_SecondPass_NullConstant(resolver, expr);
		case 27:
			return ExpressionResolver_SecondPass_Slice(resolver, expr);
		case 28:
			return ExpressionResolver_SecondPass_StringConstant(resolver, expr);
		case 29:
			return ExpressionResolver_SecondPass_Ternary(resolver, expr);
		case 30:
			return ExpressionResolver_SecondPass_ThisConstant(resolver, expr);
		case 33:
			return ExpressionResolver_SecondPass_TypeOf(resolver, expr);
		case 31:
			return ExpressionResolver_SecondPass_Variable(resolver, expr);
		default:
			Errors_ThrowNotImplemented(expr[0], "second pass for this type");
			break;
	}
	return expr;
};

let ExpressionResolver_SecondPass_BaseCtorReference = function(resolver, baseCtor) {
	baseCtor[10] = resolver[7][8][2][1][6];
	return baseCtor;
};

let ExpressionResolver_SecondPass_BinaryOp = function(resolver, expr) {
	let firstToken = expr[0];
	let opToken = expr[5];
	let op = opToken[0];
	expr[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, expr[3]);
	if (expr[4][1] == 7 && op == "is") {
		expr[4][6] = true;
	}
	expr[4] = ExpressionResolver_ResolveExpressionSecondPass(resolver, expr[4]);
	if (IsExpressionConstant(expr[3]) && IsExpressionConstant(expr[4])) {
		let intLeft = 0;
		let intRight = 0;
		let resultInt = 0;
		let floatLeft = 0.0;
		let floatRight = 0.0;
		let floatResult = 0.0;
		let isLeftNumeric = IsExpressionNumericConstant(expr[3]);
		let isRightNumeric = IsExpressionNumericConstant(expr[4]);
		let isRightZero = expr[4][8] == 0;
		if (expr[4][1] == 16) {
			isRightZero = expr[4][9] == 0;
		}
		if (isRightNumeric) {
			if (op == "/" || op == "%") {
				if (isRightZero) {
					if (op == "%") {
						Errors_Throw(opToken, "Modulo by zero");
					}
					Errors_Throw(opToken, "Division by zero");
				}
			}
		}
		if (isLeftNumeric && isRightNumeric) {
			floatLeft = GetNumericValueOfConstantExpression(expr[3]);
			floatRight = GetNumericValueOfConstantExpression(expr[4]);
			if (op == "<") {
				return Expression_createBoolConstant(firstToken, floatLeft < floatRight);
			}
			if (op == ">") {
				return Expression_createBoolConstant(firstToken, floatLeft > floatRight);
			}
			if (op == "<=") {
				return Expression_createBoolConstant(firstToken, floatLeft <= floatRight);
			}
			if (op == ">=") {
				return Expression_createBoolConstant(firstToken, floatLeft >= floatRight);
			}
			if (op == "==") {
				return Expression_createBoolConstant(firstToken, floatLeft == floatRight);
			}
			if (op == "!=") {
				return Expression_createBoolConstant(firstToken, floatLeft != floatRight);
			}
		}
		if (op == "+" && (expr[3][1] == 28 || expr[4][1] == 28)) {
			let leftStr = GetStringFromConstantExpression(expr[3]);
			let rightStr = GetStringFromConstantExpression(expr[4]);
			return Expression_createStringConstant(expr[0], leftStr + rightStr);
		}
		switch (expr[3][1] * 34 + expr[4][1]) {
			case 770:
				intLeft = expr[3][8];
				intRight = expr[4][8];
				if (op == "**") {
					return Expression_createFloatConstant(opToken, Math.pow(intLeft, intRight));
				}
				resultInt = 0;
				if (op == "+") {
					resultInt = intLeft + intRight;
				} else if (op == "-") {
					resultInt = intLeft - intRight;
				} else if (op == "*") {
					resultInt = intLeft * intRight;
				} else if (op == "/") {
					resultInt = Math.floor(intLeft / intRight);
				} else if (op == "%") {
					if (intRight < 0) {
						intRight = -intRight;
						resultInt = intLeft % intRight;
						if (resultInt > 0) {
							resultInt -= intRight;
						}
					} else {
						resultInt = intLeft % intRight;
						if (resultInt < 0) {
							resultInt += intRight;
						}
					}
				} else if (op == "|") {
					resultInt = intLeft | intRight;
				} else if (op == "&") {
					resultInt = intLeft & intRight;
				} else if (op == "^") {
					resultInt = intLeft ^ intRight;
				} else if (op == "<<") {
					resultInt = intLeft << intRight;
				} else if (op == ">>") {
					resultInt = intLeft >> intRight;
				} else if (op == ">>>") {
					fail("Not implemented");
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createIntegerConstant(opToken, resultInt);
			case 760:
				intLeft = expr[3][8];
				intRight = expr[4][8];
				if (op == "**") {
					return Expression_createFloatConstant(opToken, Math.pow(intLeft, intRight));
				}
				resultInt = 0;
				if (op == "+") {
					resultInt = intLeft + intRight;
				} else if (op == "-") {
					resultInt = intLeft - intRight;
				} else if (op == "*") {
					resultInt = intLeft * intRight;
				} else if (op == "/") {
					resultInt = Math.floor(intLeft / intRight);
				} else if (op == "%") {
					if (intRight < 0) {
						intRight = -intRight;
						resultInt = intLeft % intRight;
						if (resultInt > 0) {
							resultInt -= intRight;
						}
					} else {
						resultInt = intLeft % intRight;
						if (resultInt < 0) {
							resultInt += intRight;
						}
					}
				} else if (op == "|") {
					resultInt = intLeft | intRight;
				} else if (op == "&") {
					resultInt = intLeft & intRight;
				} else if (op == "^") {
					resultInt = intLeft ^ intRight;
				} else if (op == "<<") {
					resultInt = intLeft << intRight;
				} else if (op == ">>") {
					resultInt = intLeft >> intRight;
				} else if (op == ">>>") {
					fail("Not implemented");
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createIntegerConstant(opToken, resultInt);
			case 430:
				intLeft = expr[3][8];
				intRight = expr[4][8];
				if (op == "**") {
					return Expression_createFloatConstant(opToken, Math.pow(intLeft, intRight));
				}
				resultInt = 0;
				if (op == "+") {
					resultInt = intLeft + intRight;
				} else if (op == "-") {
					resultInt = intLeft - intRight;
				} else if (op == "*") {
					resultInt = intLeft * intRight;
				} else if (op == "/") {
					resultInt = Math.floor(intLeft / intRight);
				} else if (op == "%") {
					if (intRight < 0) {
						intRight = -intRight;
						resultInt = intLeft % intRight;
						if (resultInt > 0) {
							resultInt -= intRight;
						}
					} else {
						resultInt = intLeft % intRight;
						if (resultInt < 0) {
							resultInt += intRight;
						}
					}
				} else if (op == "|") {
					resultInt = intLeft | intRight;
				} else if (op == "&") {
					resultInt = intLeft & intRight;
				} else if (op == "^") {
					resultInt = intLeft ^ intRight;
				} else if (op == "<<") {
					resultInt = intLeft << intRight;
				} else if (op == ">>") {
					resultInt = intLeft >> intRight;
				} else if (op == ">>>") {
					fail("Not implemented");
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createIntegerConstant(opToken, resultInt);
			case 420:
				intLeft = expr[3][8];
				intRight = expr[4][8];
				if (op == "**") {
					return Expression_createFloatConstant(opToken, Math.pow(intLeft, intRight));
				}
				resultInt = 0;
				if (op == "+") {
					resultInt = intLeft + intRight;
				} else if (op == "-") {
					resultInt = intLeft - intRight;
				} else if (op == "*") {
					resultInt = intLeft * intRight;
				} else if (op == "/") {
					resultInt = Math.floor(intLeft / intRight);
				} else if (op == "%") {
					if (intRight < 0) {
						intRight = -intRight;
						resultInt = intLeft % intRight;
						if (resultInt > 0) {
							resultInt -= intRight;
						}
					} else {
						resultInt = intLeft % intRight;
						if (resultInt < 0) {
							resultInt += intRight;
						}
					}
				} else if (op == "|") {
					resultInt = intLeft | intRight;
				} else if (op == "&") {
					resultInt = intLeft & intRight;
				} else if (op == "^") {
					resultInt = intLeft ^ intRight;
				} else if (op == "<<") {
					resultInt = intLeft << intRight;
				} else if (op == ">>") {
					resultInt = intLeft >> intRight;
				} else if (op == ">>>") {
					fail("Not implemented");
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createIntegerConstant(opToken, resultInt);
			case 560:
				floatLeft = GetNumericValueOfConstantExpression(expr[3]);
				floatRight = GetNumericValueOfConstantExpression(expr[4]);
				floatResult = 0.0;
				if (op == "+") {
					floatResult = floatLeft + floatRight;
				} else if (op == "-") {
					floatResult = floatLeft - floatRight;
				} else if (op == "*") {
					floatResult = floatLeft * floatRight;
				} else if (op == "/") {
					floatResult = floatLeft / floatRight;
				} else if (op == "%") {
					if (floatRight < 0) {
						floatRight = -floatRight;
						floatResult = floatLeft % floatRight;
						if (floatResult > 0) {
							floatResult -= floatRight;
						}
					} else {
						floatResult = floatLeft % floatRight;
						if (floatResult < 0) {
							floatResult += floatRight;
						}
					}
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createFloatConstant(opToken, floatResult);
			case 764:
				floatLeft = GetNumericValueOfConstantExpression(expr[3]);
				floatRight = GetNumericValueOfConstantExpression(expr[4]);
				floatResult = 0.0;
				if (op == "+") {
					floatResult = floatLeft + floatRight;
				} else if (op == "-") {
					floatResult = floatLeft - floatRight;
				} else if (op == "*") {
					floatResult = floatLeft * floatRight;
				} else if (op == "/") {
					floatResult = floatLeft / floatRight;
				} else if (op == "%") {
					if (floatRight < 0) {
						floatRight = -floatRight;
						floatResult = floatLeft % floatRight;
						if (floatResult > 0) {
							floatResult -= floatRight;
						}
					} else {
						floatResult = floatLeft % floatRight;
						if (floatResult < 0) {
							floatResult += floatRight;
						}
					}
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createFloatConstant(opToken, floatResult);
			case 566:
				floatLeft = GetNumericValueOfConstantExpression(expr[3]);
				floatRight = GetNumericValueOfConstantExpression(expr[4]);
				floatResult = 0.0;
				if (op == "+") {
					floatResult = floatLeft + floatRight;
				} else if (op == "-") {
					floatResult = floatLeft - floatRight;
				} else if (op == "*") {
					floatResult = floatLeft * floatRight;
				} else if (op == "/") {
					floatResult = floatLeft / floatRight;
				} else if (op == "%") {
					if (floatRight < 0) {
						floatRight = -floatRight;
						floatResult = floatLeft % floatRight;
						if (floatResult > 0) {
							floatResult -= floatRight;
						}
					} else {
						floatResult = floatLeft % floatRight;
						if (floatResult < 0) {
							floatResult += floatRight;
						}
					}
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createFloatConstant(opToken, floatResult);
			case 424:
				floatLeft = GetNumericValueOfConstantExpression(expr[3]);
				floatRight = GetNumericValueOfConstantExpression(expr[4]);
				floatResult = 0.0;
				if (op == "+") {
					floatResult = floatLeft + floatRight;
				} else if (op == "-") {
					floatResult = floatLeft - floatRight;
				} else if (op == "*") {
					floatResult = floatLeft * floatRight;
				} else if (op == "/") {
					floatResult = floatLeft / floatRight;
				} else if (op == "%") {
					if (floatRight < 0) {
						floatRight = -floatRight;
						floatResult = floatLeft % floatRight;
						if (floatResult > 0) {
							floatResult -= floatRight;
						}
					} else {
						floatResult = floatLeft % floatRight;
						if (floatResult < 0) {
							floatResult += floatRight;
						}
					}
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createFloatConstant(opToken, floatResult);
			case 556:
				floatLeft = GetNumericValueOfConstantExpression(expr[3]);
				floatRight = GetNumericValueOfConstantExpression(expr[4]);
				floatResult = 0.0;
				if (op == "+") {
					floatResult = floatLeft + floatRight;
				} else if (op == "-") {
					floatResult = floatLeft - floatRight;
				} else if (op == "*") {
					floatResult = floatLeft * floatRight;
				} else if (op == "/") {
					floatResult = floatLeft / floatRight;
				} else if (op == "%") {
					if (floatRight < 0) {
						floatRight = -floatRight;
						floatResult = floatLeft % floatRight;
						if (floatResult > 0) {
							floatResult -= floatRight;
						}
					} else {
						floatResult = floatLeft % floatRight;
						if (floatResult < 0) {
							floatResult += floatRight;
						}
					}
				} else {
					ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				}
				return Expression_createFloatConstant(opToken, floatResult);
			case 974:
				if (op == "*") {
					let strExpr = expr[3];
					let intExpr = expr[4];
					if (expr[3][1] == 22) {
						strExpr = expr[4];
						intExpr = expr[3];
					}
					let size = intExpr[8];
					let val = strExpr[7];
					if (size == 0) {
						return Expression_createStringConstant(expr[0], "");
					}
					if (size == 1) {
						return strExpr;
					}
					if (val.length * size < 12) {
						let sb = [];
						let i = 0;
						while (i < size) {
							sb.push(val);
							i += 1;
						}
						return Expression_createStringConstant(expr[0], sb.join(""));
					}
					return expr;
				}
				ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				break;
			case 776:
				if (op == "*") {
					let strExpr = expr[3];
					let intExpr = expr[4];
					if (expr[3][1] == 22) {
						strExpr = expr[4];
						intExpr = expr[3];
					}
					let size = intExpr[8];
					let val = strExpr[7];
					if (size == 0) {
						return Expression_createStringConstant(expr[0], "");
					}
					if (size == 1) {
						return strExpr;
					}
					if (val.length * size < 12) {
						let sb = [];
						let i = 0;
						while (i < size) {
							sb.push(val);
							i += 1;
						}
						return Expression_createStringConstant(expr[0], sb.join(""));
					}
					return expr;
				}
				ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				break;
			default:
				ThrowOpNotDefinedError(opToken, op, expr[3][1], expr[4][1]);
				break;
		}
		fail("");
	}
	return expr;
};

let ExpressionResolver_SecondPass_BitwiseNot = function(resolver, bwn) {
	bwn[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, bwn[2]);
	if (IsExpressionConstant(bwn[2])) {
		if (bwn[2][1] != 22) {
			Errors_Throw(bwn[0], "Bitwise-NOT operator can only be applied on integers.");
		}
		return Expression_createIntegerConstant(bwn[0], -bwn[2][8] - 1);
	}
	return bwn;
};

let ExpressionResolver_SecondPass_BoolConst = function(resolver, bc) {
	return bc;
};

let ExpressionResolver_SecondPass_BoolNot = function(resolver, bn) {
	bn[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, bn[2]);
	if (IsExpressionConstant(bn[2])) {
		if (bn[2][1] != 6) {
			Errors_Throw(bn[0], "Boolean-NOT operator can only be applied to booleans.");
		}
		return Expression_createBoolConstant(bn[0], !bn[2][6]);
	}
	return bn;
};

let ExpressionResolver_SecondPass_ClassReference = function(resolver, classRef) {
	if (!classRef[6]) {
		Errors_Throw(classRef[0], "A class reference must have a field or method referenced from it.");
	}
	return classRef;
};

let ExpressionResolver_SecondPass_ConstructorReference = function(resolver, ctorRef, isExpected) {
	if (isExpected) {
		ctorRef[2][6] = true;
		ctorRef[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ctorRef[2]);
		if (ctorRef[2][1] != 7) {
			Errors_Throw(ctorRef[2][0], "This is not a valid class definition.");
		}
		ctorRef[10] = ctorRef[2][10];
		ctorRef[2] = null;
		return ctorRef;
	}
	Errors_Throw(ctorRef[0], "A constructor must be immediately invoked.");
	return ctorRef;
};

let ExpressionResolver_SecondPass_DictionaryDefinition = function(resolver, dictDef) {
	let length = dictDef[13].length;
	let strCollide = {};
	let intCollide = {};
	let i = 0;
	while (i < length) {
		let key = ExpressionResolver_ResolveExpressionSecondPass(resolver, dictDef[13][i]);
		let isMixed = false;
		let isCollide = false;
		if (key[1] == 22) {
			isMixed = Object.keys(strCollide).length > 0;
			isCollide = intCollide[key[8]] !== undefined;
			intCollide[key[8]] = true;
		} else if (key[1] == 28) {
			isMixed = Object.keys(intCollide).length > 0;
			isCollide = strCollide[key[7]] !== undefined;
			strCollide[key[7]] = true;
		} else {
			Errors_Throw(key[0], "This type of expression cannot be used as a dictionary key. Dictionary keys must be constant integer or string expressions.");
		}
		if (isMixed) {
			Errors_Throw(key[0], "Dictionary cannot contain mixed types for keys.");
		}
		if (isCollide) {
			Errors_Throw(key[0], "There are multiple keys with this same value.");
		}
		dictDef[13][i] = key;
		dictDef[14][i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, dictDef[14][i]);
		i += 1;
	}
	return dictDef;
};

let ExpressionResolver_SecondPass_DotField = function(resolver, df) {
	if (df[2][1] == 7) {
		df[2][6] = true;
	}
	df[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, df[2]);
	switch (df[2][1]) {
		case 28:
			if (df[7] == "length") {
				return Expression_createIntegerConstant(df[0], df[2][7].length);
			}
			break;
		case 30:
			break;
		case 1:
			break;
		case 7:
			let classDef = df[2][10][2];
			let member = null;
			if (classDef[5][df[7]] !== undefined) {
				member = classDef[5][df[7]];
				if (!member[7]) {
					Errors_Throw(df[5], [classDef[6][5], ".", df[7], " is not static."].join(''));
				}
			} else {
				Errors_Throw(df[5], ["The class ", classDef[6][5], " does not have a member named '.", df[7], "'."].join(''));
			}
			break;
	}
	return df;
};

let ExpressionResolver_SecondPass_EnumConstant = function(resolver, enumConst) {
	return enumConst;
};

let ExpressionResolver_SecondPass_ExtensionInvocation = function(resolver, expr) {
	ExpressionResolver_ResolveExpressionArraySecondPass(resolver, expr[12]);
	let argc = -1;
	if (SpecialActionUtil_IsSpecialActionAndNotExtension(resolver[0][2], expr[7])) {
		argc = SpecialActionUtil_GetSpecialActionArgc(resolver[0][2], expr[7]);
	} else {
		let name = expr[7];
		if (name == "delay_invoke") {
			argc = 2;
		} else if (name == "io_stdout") {
			argc = 1;
		} else if (name == "sleep") {
			argc = 1;
		} else if (Resolver_isValidRegisteredExtension(resolver, expr[7])) {
			return expr;
		} else {
			Errors_Throw(expr[0], "Extension is not registered: " + expr[7]);
		}
	}
	if (argc != -1 && expr[12].length != argc) {
		Errors_Throw(expr[0], "Incorrect number of arguments to extension");
	}
	return expr;
};

let ExpressionResolver_SecondPass_FloatConstant = function(resolver, floatConst) {
	return floatConst;
};

let ExpressionResolver_SecondPass_FunctionInvocation = function(resolver, funcInvoke) {
	if (funcInvoke[2][1] == 9) {
		let ctorRef = ExpressionResolver_SecondPass_ConstructorReference(resolver, funcInvoke[2], true);
		if (ctorRef[1] != 9) {
			fail("");
		}
		ExpressionResolver_ResolveExpressionArraySecondPass(resolver, funcInvoke[12]);
		return Expression_createConstructorInvocation(funcInvoke[0], ctorRef[10], funcInvoke[5], funcInvoke[12]);
	}
	funcInvoke[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, funcInvoke[2]);
	switch (funcInvoke[2][1]) {
		case 31:
			break;
		case 1:
			break;
		case 18:
			break;
		case 11:
			break;
		case 2:
			break;
		case 20:
			break;
		default:
			Errors_Throw(funcInvoke[5], "Cannot invoke this type of expression like a function.");
			break;
	}
	ExpressionResolver_ResolveExpressionArraySecondPass(resolver, funcInvoke[12]);
	return funcInvoke;
};

let ExpressionResolver_SecondPass_FunctionReference = function(resolver, funcRef) {
	return funcRef;
};

let ExpressionResolver_SecondPass_ImportReference = function(resolver, importRef) {
	Errors_Throw(importRef[0], "An import reference cannot be passed as a reference. You must reference the imported entity directly.");
	return null;
};

let ExpressionResolver_SecondPass_Index = function(resolver, indexExpr) {
	indexExpr[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, indexExpr[2]);
	indexExpr[4] = ExpressionResolver_ResolveExpressionSecondPass(resolver, indexExpr[4]);
	return indexExpr;
};

let ExpressionResolver_SecondPass_InlineIncrement = function(resolver, inlineIncr) {
	inlineIncr[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, inlineIncr[2]);
	switch (inlineIncr[2][1]) {
		case 31:
			break;
		case 20:
			break;
		case 11:
			break;
		default:
			Errors_Throw(inlineIncr[5], ["Cannot use the '", inlineIncr[5][0], "' operator on this type of expression."].join(''));
			break;
	}
	return inlineIncr;
};

let ExpressionResolver_SecondPass_IntegerConstant = function(resolver, intConst) {
	return intConst;
};

let ExpressionResolver_SecondPass_Lambda = function(resolver, lambda) {
	return lambda;
};

let ExpressionResolver_SecondPass_ListDefinition = function(resolver, listDef) {
	let i = 0;
	while (i < listDef[14].length) {
		listDef[14][i] = ExpressionResolver_ResolveExpressionSecondPass(resolver, listDef[14][i]);
		i += 1;
	}
	return listDef;
};

let ExpressionResolver_SecondPass_NamespaceReference = function(resolver, nsRef) {
	Errors_Throw(nsRef[0], "You cannot use a namespace reference like this.");
	return null;
};

let ExpressionResolver_SecondPass_NegativeSign = function(resolver, negSign) {
	let root = ExpressionResolver_ResolveExpressionSecondPass(resolver, negSign[2]);
	negSign[2] = root;
	if (IsExpressionNumericConstant(root)) {
		switch (root[1]) {
			case 22:
				root[8] *= -1;
				break;
			case 16:
				root[9] *= -1;
				break;
			case 12:
				root = Expression_createIntegerConstant(root[0], -root[8]);
				break;
			default:
				fail("Not implemented");
				break;
		}
		return root;
	}
	return negSign;
};

let ExpressionResolver_SecondPass_NullConstant = function(resolver, nullConst) {
	return nullConst;
};

let ExpressionResolver_SecondPass_Slice = function(resolver, slice) {
	slice[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, slice[2]);
	let i = 0;
	while (i < 3) {
		if (slice[12][i] != null) {
			let expr = ExpressionResolver_ResolveExpressionSecondPass(resolver, slice[12][i]);
			slice[12][i] = expr;
			if (IsExpressionConstant(expr)) {
				if (expr[1] != 22 && expr[1] != 12) {
					Errors_Throw(expr[0], "Only integers may be used in a slice expression.");
				}
			}
		}
		i++;
	}
	return slice;
};

let ExpressionResolver_SecondPass_StringConstant = function(resolver, strConst) {
	return strConst;
};

let ExpressionResolver_SecondPass_Ternary = function(resolver, ternary) {
	ternary[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary[2]);
	ternary[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary[3]);
	ternary[4] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ternary[4]);
	if (IsExpressionConstant(ternary[2])) {
		if (ternary[2][1] != 5) {
			Errors_Throw(ternary[2][0], "Only booleans can be used as ternary conditions.");
		}
		if (ternary[2][6]) {
			return ternary[3];
		}
		return ternary[4];
	}
	return ternary;
};

let ExpressionResolver_SecondPass_ThisConstant = function(resolver, thisExpr) {
	if (resolver[7][8] == null) {
		fail("not implemented");
	}
	return thisExpr;
};

let ExpressionResolver_SecondPass_TypeOf = function(resolver, typeofExpr) {
	typeofExpr[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, typeofExpr[2]);
	let stringConst = null;
	switch (typeofExpr[2][1]) {
		case 22:
			stringConst = "int";
			break;
		case 16:
			stringConst = "float";
			break;
		case 26:
			stringConst = "null";
			break;
		case 5:
			stringConst = "bool";
			break;
		case 28:
			stringConst = "string";
			break;
		case 18:
			stringConst = "function";
			break;
		case 24:
			if (typeofExpr[2][14].length == 0) {
				stringConst = "list";
			}
			break;
		case 10:
			if (typeofExpr[2][13].length == 0) {
				stringConst = "dict";
			}
			break;
	}
	if (stringConst != null) {
		return Expression_createStringConstant(typeofExpr[0], stringConst);
	}
	return typeofExpr;
};

let ExpressionResolver_SecondPass_Variable = function(resolver, varExpr) {
	if (varExpr[7] == "print") {
		fail("");
	}
	if (resolver[7][2][5][varExpr[7]] !== undefined) {
		return varExpr;
	}
	Errors_Throw(varExpr[0], ["There is no variable by the name of '", varExpr[7], "'."].join(''));
	return null;
};

let ExpressionResolver_WrapEntityIntoReferenceExpression = function(resolver, token, entity) {
	switch (entity[1]) {
		case 6:
			return Expression_createFunctionReference(token, entity[3], entity);
		case 1:
			return Expression_createClassReference(token, entity);
		case 2:
			return Expression_cloneWithNewToken(token, entity[2][0]);
		case 4:
			return Expression_createEnumReference(token, entity);
		case 7:
			return Expression_createNamespaceReference(token, entity);
	}
	Errors_Throw(token, "Not implemented!");
	return null;
};

let fail = function(msg) {
	let failArgs = PST$createNewArray(1);
	failArgs[0] = msg;
	return (PST$extCallbacks["hardCrash"] || ((o) => null))(failArgs);
};

let FieldEntity_new = function(fieldToken, nameToken, equalToken, defaultValueOrNull) {
	let fe = [defaultValueOrNull, equalToken, null];
	fe[2] = AbstractEntity_new(fieldToken, 5, fe);
	fe[2][4] = nameToken;
	fe[2][3] = nameToken[0];
	return fe;
};

let FileContext_initializeImportLookup = function(fileCtx) {
	fileCtx[5] = {};
	let i = 0;
	while (i < fileCtx[4].length) {
		let imp = fileCtx[4][i];
		let varName = null;
		if (imp[3] != null) {
			varName = imp[3][0];
		} else {
			if (imp[2].indexOf(".") != -1) {
				Errors_Throw(imp[0], "Dot-delimited import paths must use an alias.");
			}
			varName = imp[2];
		}
		if (varName != "*" && fileCtx[5][varName] !== undefined) {
			Errors_Throw(imp[3], ["There are multiple imports loaded as the variable '", varName, "'"].join(''));
		}
		fileCtx[5][varName] = imp;
		i += 1;
	}
};

let FileContext_new = function(staticCtx, path, content) {
	return [staticCtx, path, content, TokenStream_new(path, Tokenize(path, content, staticCtx)), null, null, false, false, null];
};

let finalizeBreakContinue = function(originalBuffer, additionalBreakOffset, allowContinue, additionalContinueOffset) {
	let i = 0;
	let rows = flatten(originalBuffer);
	i = 0;
	while (i < rows.length) {
		let op = rows[i][0];
		if (op == -1 || op == -2) {
			let additionalOffset = additionalBreakOffset;
			if (op == -2) {
				if (!allowContinue) {
					fail("");
				}
				additionalOffset = additionalContinueOffset;
			}
			rows[i][0] = 24;
			let offsetToEnd = rows.length - i - 1 + additionalOffset;
			rows[i][3] = PST$createNewArray(1);
			rows[i][3][0] = offsetToEnd;
		}
		i += 1;
	}
	let output = null;
	i = 0;
	while (i < rows.length) {
		output = join2(output, ByteCodeBuffer_fromRow(rows[i]));
		i += 1;
	}
	return output;
};

let flatten = function(buffer) {
	if (buffer == null) {
		return PST$createNewArray(0);
	}
	let q = [];
	q.push(buffer);
	let output = [];
	while (q.length > 0) {
		let current = q[q.length - 1];
		q.splice(q.length - 1, 1);
		if (current[1]) {
			output.push(current[4]);
		} else {
			q.push(current[3]);
			q.push(current[2]);
		}
	}
	return [...(output)];
};

let FlattenBinaryOpChain = function(expressions, ops) {
	let i = 0;
	let opType = ops[0][0];
	let isShortCircuit = opType == "??" || opType == "&&" || opType == "||";
	let acc = null;
	let length = expressions.length;
	if (isShortCircuit) {
		acc = Expression_createBinaryOp(expressions[length - 2], ops[length - 2], expressions[length - 1]);
		i = length - 3;
		while (i >= 0) {
			acc = Expression_createBinaryOp(expressions[i], ops[i], acc);
			i -= 1;
		}
	} else {
		acc = Expression_createBinaryOp(expressions[0], ops[0], expressions[1]);
		i = 2;
		while (i < length) {
			acc = Expression_createBinaryOp(acc, ops[i - 1], expressions[i]);
			i += 1;
		}
	}
	return acc;
};

let FlattenEntities = function(staticCtx, rootEntities) {
	let output = [];
	let queue = [];
	let arrTemp = Object.values(rootEntities);
	let j = 0;
	while (j < arrTemp.length) {
		queue.push(arrTemp[j]);
		j += 1;
	}
	let i = 0;
	while (i < queue.length) {
		let entity = queue[i];
		output.push(entity);
		let lookupMembers = Object.values(Entity_getMemberLookup(staticCtx, entity));
		j = 0;
		while (j < lookupMembers.length) {
			let mem = lookupMembers[j];
			queue.push(mem);
			j += 1;
		}
		i += 1;
	}
	return [...(output)];
};

let FloatToStringWorkaround = function(val) {
	let str = val + '';
	str = str.toLowerCase();
	if (str.indexOf("e") != -1 && str.endsWith(".0")) {
		str = str.substring(0, 0 + (str.length - 2)).split("e").join(".0e");
	}
	return str;
};

let FunctionEntity_BuildConstructor = function(ctorToken, args, argDefaultValues, baseArgs, code, isStatic) {
	let fle = FunctionEntity_new(ctorToken, 3, args, argDefaultValues, code);
	if (isStatic) {
		fle[6] = 5;
		fle[3][3] = "@cctor";
	} else {
		fle[6] = 3;
		fle[3][3] = "@ctor";
	}
	if (baseArgs != null) {
		fle[2] = [...(baseArgs)];
	}
	fle[3][7] = isStatic;
	return fle;
};

let FunctionEntity_BuildLambda = function(ctx, firstToken, argNames, argDefaultValues, code) {
	let fle = FunctionEntity_new(firstToken, 10, argNames, argDefaultValues, code);
	fle[6] = 6;
	fle[3][9] = ctx;
	return fle;
};

let FunctionEntity_BuildMethodOrStandalone = function(funcToken, nameToken, args, argValues, code, isStatic, classParent) {
	let isMethod = classParent != null;
	let fle = FunctionEntity_new(funcToken, 6, args, argValues, code);
	if (isMethod) {
		if (isStatic) {
			fle[6] = 4;
		} else {
			fle[6] = 2;
		}
	} else {
		fle[6] = 1;
	}
	fle[3][4] = nameToken;
	fle[3][3] = nameToken[0];
	return fle;
};

let FunctionEntity_new = function(firstToken, type, argNames, argDefaultValues, code) {
	let fn = [[...(argNames)], [...(argDefaultValues)], null, null, [...(code)], null, -1];
	fn[3] = AbstractEntity_new(firstToken, type, fn);
	return fn;
};

let GEN_BUILTINS_base64 = function() {
	return "@5base64ToBytes(b64str) { @4$b64_to_bytes(b64str); }\n@5base64ToUtf8String(b64str) { @4$txt_bytes_to_string($b64_to_bytes(b64str),'utf8'); }\n@5bytesToBase64(bytes, web=false) { @4$b64_from_bytes(bytes, web == true); }\n@5stringToBase64(str, web=false) { @4$b64_from_bytes($txt_string_to_bytes(str,'utf8'), web == true); }";
};

let GEN_BUILTINS_builtins = function() {
	return "@5print(o) { @4$io_stdout(o); }\n@5tryParseInt(s) { @4$parse_int(s); }\n@5tryParseFloat(s) { @4$parse_float(s); }\n@5floor(n) { @4$math_floor(n); }\n@5getUnixTime() { @4$unix_time(0); }\n@5getUnixTimeFloat() { @4$unix_time(1); }\n@5delayInvoke(fn, sec) { $delay_invoke(fn, 1.0*sec); }\n@5sleep(sec) { $sleep(1.0*sec); }\n@1map(a, f) {\no = [];\ns = a.length;\nfor (i = 0; i < s; i++) o.add(f(a[i]));\n@4o;\n}\n@1filter(a, f) {\no = [];\ns = a.length;\nfor (i = 0; i < s; i++) {\nv = a[i];\nif (f(v) == true) o.add(v);\n}\n@4o;\n}\n@1kvpMap(d, f) {\nk=d.keys();\ns=k.length;\nif (s>0) {\nv=d.values();\no=[];\nfor (i=0;i<s;i++){\nk[i]=f(k[i],v[i]);\n}\n}\n@4k;\n}\n@1reduce(a, f, c = reduce) {\ns = a.length;\nif (c == reduce) {\nif (s < 1) thrw(2, \"List must have at least one item.\");\nc = a[0];\ni = 1;\n} else\ni = 0;\nwhile (i < s) {\nc = f(c, a[i]);\ni++;\n}\n@4c;\n}\n@3Exception { field message; field trace = []; constructor(msg = null) { this.message = (msg ?? '') + ''; } }\n@3Fatal@6@2\n@3FieldNotFound@6@2\n@3InvalidArgument@6@2\n@3Invocation@6@2\n@3KeyNotFound@6@2\n@3NotImplemented@6@2\n@3NullReference@6@2\n@3OutOfRange@6@2\n@3StackOverflowException : FatalException@2\n@3Type@6@2\n@3ZeroDivisor@6@2\n@3ImmutableData@6@2\n@1thrw(n, m) {\nswitch (n) {\ncase 1: throw new FatalException(m);\ncase 2: throw new StackOverflowException(m);\ncase 3: throw new FieldNotFoundException(m);\ncase 4: throw new InvalidArgumentException(m);\ncase 5: throw new InvocationException(m);\ncase 6: throw new KeyNotFoundException(m);\ncase 7: throw new NullReferenceException(m);\ncase 8: throw new OutOfRangeException(m);\ncase 9: throw new TypeException(m);\ncase 10: throw new ZeroDivisorException(m);\ncase 11: throw new ImmutableDataException(m);\n}\nthrow new Exception(m);\n}\n@1sortK(arr, keyFn) {\nkeys = [];\nfor (i = 0; i < arr.length; i++) {\nkeys.add(keyFn(arr[i]));\n}\n@4sort(keys, sort, arr);\n}\n@1sort(arr, cmp=sort, mirror=null) {\nis_sys_cmp = cmp == sort;\no = $sort_start(arr, mirror);\np = [0, 0];\nwhile ($sort_get_next_cmp(o, p)) {\n$sort_proceed(o, (is_sys_cmp ? $cmp(p[0], p[1]) : cmp(p[0], p[1])) > 0);\n}\n@4$sort_end(o);\n}";
};

let GEN_BUILTINS_json = function() {
	return "@3JsonParse@6 {\nconstructor(l, c) : base('JSON parse error on line ' + l + ', col ' + c) {}\n}\n@3JsonSerialization@6 {\nconstructor() : base('Value contained un-serialiazable value.') {}\n}\n@5jsonParse(str) { @4parseImpl(str, true); }\n@5tryJsonParse(str) { @4parseImpl(str, false); }\n@1parseImpl(str, f) {\ne = [0, 0, 0];\nv = $json_parse(str + '', e);\nif (e[0] < 1) @4v;\nif (f) throw new JsonParseException(e[1], e[2]);\n@4null;\n}\n@5jsonSerialize(obj, pretty = false, tab = 2) {\np = pretty == true;\ns = $json_serialize(obj, p);\nif (s == null) throw new JsonSerializationException();\n@4(p && tab != '\\t') ? s.replace('\\t', ' ' * tab) : s;\n}";
};

let GEN_BUILTINS_math = function() {
	return "@5sin(r) { @4$math_sin(r - 0); }\n@5cos(r) { @4$math_cos(r - 0); }\n@5tan(r) { @4$math_tan(r - 0); }\n@5arcsin(x) { @4$math_arcsin(x - 0); }\n@5arccos(x) { @4$math_arccos(x - 0); }\n@5arctan(yOrVal, x = null) { @4$math_arctan(yOrVal, x == null ? null : (x - 0)); }\n@5log10(val) { @4$math_log(val - 0, 0); }\n@5log2(val) { @4$math_log(val - 0, 1); }\n@5ln(val) { @4$math_log(val - 0, -1); }\n@5abs(val) { @4val < 0 ? -val : val; }\n@5sign(val) { @4val == 0 ? 0 : val < 0 ? -1 : 1; }\n@5sqrt(val) { @4val ** .5; }";
};

let GEN_BUILTINS_random = function() {
	return "@5randomFloat() { @4$random_float(); }\n@5randomBool() { @4$random_float() < .5; }\n@5randomInt(a, b = null) {\nif (b == null) {\nb = a;\na = 0;\n}\nd = b - a;\nif (d <= 0) throw new InvalidArgumentException(\"Range must be a positive number.\");\n@4a + $math_floor($random_float() * d);\n}";
};

let GEN_BUILTINS_textencoding = function() {
	return "@3TextEncoding@6 {\nconstructor(m) : base(m) { }\n}\n@1validEnc(e) {\ne = (e ?? '') + '';\nt = e.lower().replace('-', '');\nif ($txt_is_valid_enc(t)) @4t;\nthrow new TextEncodingException(\"'\" + e + \"' is not a valid encoding.\");\n}\n@5bytesToText(arr, e='utf8') {\nn = validEnc(e);\ns = $txt_bytes_to_string(arr, n);\nif (s == null) throw new TextEncodingException(\"Invalid byte values for encoding: '\" + n + \"'\");\n@4s;\n}\n@5textToBytes(s, e = 'utf8') {\n@4$txt_string_to_bytes((s ?? '') + '', validEnc(e));\n}";
};

let GEN_BUILTINS_xml = function() {
	return "@3XmlNode{}\n@3XmlElement:XmlNode{\nfield name;\nfield attributes;\nfield children = [];\nconstructor() : base() { }\n}\n@3XmlText:XmlNode {\nfield value;\nconstructor() : base() { }\n}\n@3XmlParse@6 {\nfield line;\nfield col;\nfield err;\nconstructor(e, l, c) : base('XML Parse Error: ' + e) {\nthis.err = e;\nthis.line = l;\nthis.col = c;\n}\n}\n@5parseXml(s) {\no = [0, 0, 0, 0];\n$xml_parse(s + '', o);\nif(o[0] == 0)\nthrow new XmlParseException(o[1], o[2], o[3]);\n@4_build([0], o[1]);\n}\n@1_build(\ni,\nb\n) {\nif (b[i[0]] == 1) {\ni[0]++;\no = new XmlElement();\no.name = b[i[0]++];\nc = b[i[0]++];\na = { };\nwhile (c --> 0) {\nk = b[i[0]++];\nv = b[i[0]++];\na[k] = v;\n}\no.attributes = a;\nc = b[i[0]++];\nwhile (c --> 0)\no.children.add(_build(i, b));\n} else {\no = new XmlText();\no.value = b[i[0]++];\n}\n@4o;\n}";
};

let GetBuiltinRawStoredString = function(m) {
	if (m == "builtins") {
		return GEN_BUILTINS_builtins();
	}
	if (m == "base64") {
		return GEN_BUILTINS_base64();
	}
	if (m == "json") {
		return GEN_BUILTINS_json();
	}
	if (m == "math") {
		return GEN_BUILTINS_math();
	}
	if (m == "random") {
		return GEN_BUILTINS_random();
	}
	if (m == "textencoding") {
		return GEN_BUILTINS_textencoding();
	}
	if (m == "xml") {
		return GEN_BUILTINS_xml();
	}
	return null;
};

let getDeterministOrderOfModules = function(modules) {
	let i = 0;
	let lookup = {};
	i = 0;
	while (i < modules.length) {
		lookup[modules[i][0]] = modules[i];
		i += 1;
	}
	let keys = PST$sortedCopyOfArray(Object.keys(lookup));
	let output = [];
	i = 0;
	while (i < keys.length) {
		output.push(lookup[keys[i]]);
		i += 1;
	}
	return [...(output)];
};

let GetNumericValueOfConstantExpression = function(exprConst) {
	if (exprConst[1] == 16) {
		return exprConst[9];
	}
	return exprConst[8] + 0.0;
};

let GetSourceForBuiltinModule = function(moduleName) {
	let code = GetBuiltinRawStoredString(moduleName);
	if (code == null) {
		fail(moduleName);
		return null;
	}
	return code.split("@1").join("function ").split("@2").join(" { constructor(m=null):base(m){} }").split("@3").join("@public class ").split("@4").join("return ").split("@5").join("@public function ").split("@6").join("Exception : Exception");
};

let GetStringFromConstantExpression = function(expr) {
	if (expr[1] == 28) {
		return expr[7];
	}
	if (expr[1] == 22) {
		return expr[8] + '';
	}
	if (expr[1] == 5) {
		if (expr[6]) {
			return "true";
		}
		return "false";
	}
	if (expr[1] == 26) {
		return "null";
	}
	if (expr[1] == 12) {
		return expr[7];
	}
	if (expr[1] == 16) {
		let val = FloatToStringWorkaround(expr[9]);
		if (val.toLowerCase().indexOf("e") != -1) {
			fail("Not implemented");
		}
		if (val.indexOf(",") != -1) {
			val = val.split(",").join(".");
		}
		if (!(val.indexOf(".") != -1)) {
			val += ".0";
		}
		return val;
	}
	fail("Not implemented");
	return null;
};

let ImportParser_AdvanceThroughImports = function(tokens, isCoreBuiltin) {
	let output = [];
	if (!isCoreBuiltin) {
		output.push(ImportParser_createBuiltinImport(tokens));
	}
	while (Tokens_hasMore(tokens) && Tokens_isNext(tokens, "import")) {
		let importToken = Tokens_popKeyword(tokens, "import");
		let tokenChain = [];
		tokenChain.push(Tokens_popName(tokens, "module name"));
		while (Tokens_popIfPresent(tokens, ".")) {
			tokenChain.push(Tokens_popName(tokens, "module name"));
		}
		let importTargetName = null;
		if (Tokens_popIfPresent(tokens, "->")) {
			if (Tokens_isNext(tokens, "*")) {
				importTargetName = Tokens_pop(tokens);
			} else {
				importTargetName = Tokens_popName(tokens, "import target variable");
			}
		}
		Tokens_popExpected(tokens, ";");
		output.push(ImportStatement_new(importToken, tokenChain, importTargetName));
	}
	return [...(output)];
};

let ImportParser_createBuiltinImport = function(tokens) {
	let builtinName = [];
	builtinName.push(createFakeToken(tokens, 2, "{BUILTIN}", 0, 0));
	return ImportStatement_new(createFakeToken(tokens, 1, "import", 0, 0), builtinName, createFakeToken(tokens, 3, "*", 0, 0));
};

let ImportStatement_new = function(importToken, tokenChain, targetVarName) {
	let flatName = [];
	let i = 0;
	while (i < tokenChain.length) {
		flatName.push(tokenChain[i][0]);
		i += 1;
	}
	return [importToken, [...(tokenChain)], flatName.join("."), targetVarName, targetVarName != null && targetVarName[0] == "*", null];
};

let IsBuiltInModule = function(moduleId) {
	return moduleId != "builtins" && GetBuiltinRawStoredString(moduleId) != null;
};

let IsExpressionConstant = function(expr) {
	switch (expr[1]) {
		case 5:
			return true;
		case 26:
			return true;
		case 22:
			return true;
		case 16:
			return true;
		case 28:
			return true;
		case 12:
			return true;
	}
	return false;
};

let IsExpressionNumericConstant = function(expr) {
	let t = expr[1];
	return t == 22 || t == 16 || t == 12;
};

let join2 = function(a, b) {
	if (a == null) {
		return b;
	}
	if (b == null) {
		return a;
	}
	return ByteCodeBuffer_from2(a, b);
};

let join3 = function(a, b, c) {
	return join2(a, join2(b, c));
};

let join4 = function(a, b, c, d) {
	return join2(join2(a, b), join2(c, d));
};

let join5 = function(a, b, c, d, e) {
	return join2(join2(a, b), join3(c, d, e));
};

let join6 = function(a, b, c, d, e, f) {
	return join3(join2(a, b), join2(c, d), join2(e, f));
};

let join7 = function(a, b, c, d, e, f, g) {
	return join4(join2(a, b), join2(c, d), join2(e, f), g);
};

let LookupUtil_DoFirstPassVariableLookupThroughImports = function(ctx, refToken, name) {
	let fileCtx = ctx[7][9];
	if (fileCtx[5][name] !== undefined) {
		return Expression_createImportReference(refToken, fileCtx[5][name]);
	}
	let imports = fileCtx[4];
	let i = imports.length - 1;
	while (i >= 0) {
		let importStatement = imports[i];
		if (importStatement[4]) {
			let mod = importStatement[5];
			let referenceExpression = LookupUtil_tryCreateModuleMemberReference(mod, refToken, name);
			if (referenceExpression != null) {
				return referenceExpression;
			}
		}
		i -= 1;
	}
	return null;
};

let LookupUtil_DoLookupForName = function(resolver, throwToken, name) {
	if (resolver[3][name] !== undefined) {
		return resolver[3][name];
	}
	if (resolver[7][9][5][name] !== undefined) {
		return ModuleWrapperEntity_new(throwToken, resolver[7][9][5][name])[1];
	}
	let walker = resolver[7];
	while (walker != null) {
		let lookup = Entity_getMemberLookup(resolver[0], walker);
		if (lookup[name] !== undefined) {
			return lookup[name];
		}
		walker = walker[8];
	}
	let importStatements = resolver[7][9][4];
	let i = 0;
	while (i < importStatements.length) {
		let impStmnt = importStatements[i];
		if (impStmnt[4]) {
			if (impStmnt[5][2][name] !== undefined) {
				return impStmnt[5][2][name];
			}
		}
		i += 1;
	}
	return null;
};

let LookupUtil_tryCreateModuleMemberReference = function(mod, refToken, name) {
	if (mod[2][name] !== undefined) {
		let tle = mod[2][name];
		switch (tle[1]) {
			case 6:
				return Expression_createFunctionReference(refToken, name, tle);
			case 2:
				return tle[2][0];
			case 1:
				return Expression_createClassReference(refToken, tle);
			case 7:
				return Expression_createNamespaceReference(refToken, tle);
			default:
				fail("Not implemented");
				break;
		}
	}
	return null;
};

let ModuleWrapperEntity_new = function(token, imp) {
	let modEnts = imp[5][2];
	let mw = [modEnts, null];
	mw[1] = AbstractEntity_new(token, 9, mw);
	return mw;
};

let NamespaceEntity_new = function(nsToken, nameToken, fqName) {
	let ns = [{}, null];
	ns[1] = AbstractEntity_new(nsToken, 7, ns);
	ns[1][4] = nameToken;
	ns[1][3] = nameToken[0];
	ns[1][5] = fqName;
	return ns;
};

let OrderStringsByDescendingFrequencyUsingLookup = function(frequencyLookupByKey) {
	let total = 0;
	let i = 0;
	let values = Object.keys(frequencyLookupByKey);
	i = 0;
	while (i < values.length) {
		total += frequencyLookupByKey[values[i]];
		i += 1;
	}
	let padSize = (total + '').length + 1;
	let valueByLexicalSortKey = {};
	i = 0;
	while (i < values.length) {
		let value = values[i];
		let key = [PadIntegerToSize(total - frequencyLookupByKey[value], padSize), ":", value].join('');
		valueByLexicalSortKey[key] = value;
		i += 1;
	}
	let keys = PST$sortedCopyOfArray(Object.keys(valueByLexicalSortKey));
	let output = [];
	i = 0;
	while (i < keys.length) {
		output.push(valueByLexicalSortKey[keys[i]]);
		i += 1;
	}
	return [...(output)];
};

let PadIntegerToSize = function(n, size) {
	let o = n + '';
	while (o.length < size) {
		o = "0" + o;
	}
	return o;
};

let ParseAddition = function(tokens) {
	let root = ParseMultiplication(tokens);
	if (Tokens_doesNextInclulde2(tokens, "+", "-")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_doesNextInclulde2(tokens, "+", "-")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseMultiplication(tokens));
		}
		root = FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseAnnotations = function(compCtx, tokens) {
	let output = {};
	while (Tokens_peekType(tokens) == 8) {
		let token = Tokens_pop(tokens);
		let annotationName = token[0].substring(1, 1 + (token[0].length - 1));
		if (output[annotationName] !== undefined) {
			Errors_Throw(token, "Multiplie redundant annotations.");
		}
		if (!StringSet_has(compCtx[0][3], annotationName)) {
			Errors_Throw(token, ["Unrecognized annotation: '@", annotationName, "'"].join(''));
		}
		output[annotationName] = token;
	}
	return output;
};

let ParseAnyForLoop = function(tokens) {
	if (!Tokens_isNext(tokens, "for")) {
		Tokens_popKeyword(tokens, "for");
	}
	let openParen = Tokens_peekAhead(tokens, 1);
	let varName = Tokens_peekAhead(tokens, 2);
	let colon = Tokens_peekAhead(tokens, 3);
	if (colon != null && openParen[0] == "(" && varName[2] == 2 && colon[0] == ":") {
		return ParseForEachLoop(tokens);
	}
	return ParseTraditionalForLoop(tokens);
};

let ParseArgDefinitionList = function(tokens, tokensOut, defaultValuesOut) {
	Tokens_popExpected(tokens, "(");
	while (!Tokens_popIfPresent(tokens, ")")) {
		if (tokensOut.length > 0) {
			Tokens_popExpected(tokens, ",");
		}
		tokensOut.push(Tokens_popName(tokens, "argument name"));
		let argValue = null;
		if (Tokens_popIfPresent(tokens, "=")) {
			argValue = ParseExpression(tokens);
		}
		defaultValuesOut.push(argValue);
	}
};

let ParseAtomicExpression = function(tokens) {
	let nextToken = Tokens_peek(tokens);
	if (nextToken == null) {
		Tokens_ensureMore(tokens);
	}
	let next = nextToken[0];
	switch (nextToken[2]) {
		case 3:
			if (next == "(") {
				if (Tokens_isSequenceNext3(tokens, "(", ")", "=>")) {
					return ParseLambda(tokens);
				}
				if (Tokens_isSequenceNext3(tokens, "(", null, ",") && Tokens_peekAhead(tokens, 1)[2] == 2) {
					return ParseLambda(tokens);
				}
				if (Tokens_isSequenceNext3(tokens, "(", null, "=") && Tokens_peekAhead(tokens, 1)[2] == 2) {
					return ParseLambda(tokens);
				}
				if (Tokens_isSequenceNext4(tokens, "(", null, ")", "=>")) {
					return ParseLambda(tokens);
				}
				Tokens_pop(tokens);
				let expr = ParseExpression(tokens);
				Tokens_popExpected(tokens, ")");
				return expr;
			}
			if (next == "[") {
				return ParseListDefinition(tokens);
			}
			if (next == "{") {
				return ParseDictionaryDefinition(tokens);
			}
			if (next == "$") {
				let builtinPrefix = Tokens_pop(tokens);
				let builtinName = Tokens_popName(tokens, "built-in function name");
				return Expression_createExtensionReference(builtinPrefix, builtinName[0]);
			}
			break;
		case 1:
			if (next == "true" || next == "false") {
				let boolTok = Tokens_pop(tokens);
				return Expression_createBoolConstant(boolTok, next == "true");
			}
			if (next == "null") {
				return Expression_createNullConstant(Tokens_pop(tokens));
			}
			if (next == "new") {
				let newTok = Tokens_pop(tokens);
				let nameChain = [];
				nameChain.push(Tokens_popName(tokens, "class name"));
				while (Tokens_isNext(tokens, ".")) {
					nameChain.push(Tokens_pop(tokens));
					nameChain.push(Tokens_popName(tokens, "class name"));
				}
				let ctorChain = Expression_createVariable(nameChain[0], nameChain[0][0]);
				let i = 1;
				while (i < nameChain.length) {
					ctorChain = Expression_createDotField(ctorChain, nameChain[i], nameChain[i + 1][0]);
					i += 2;
				}
				return Expression_createConstructorReference(newTok, ctorChain);
			}
			if (next == "this") {
				return Expression_createThisReference(Tokens_pop(tokens));
			}
			if (next == "base") {
				return Expression_createBaseReference(Tokens_pop(tokens));
			}
			break;
		case 5:
			let intVal = TryParseInteger(nextToken, next, false);
			return Expression_createIntegerConstant(Tokens_pop(tokens), intVal);
		case 7:
			let floatVal = TryParseFloat(nextToken, next);
			return Expression_createFloatConstant(Tokens_pop(tokens), floatVal);
		case 6:
			let intValHex = TryParseInteger(nextToken, next, true);
			return Expression_createIntegerConstant(Tokens_pop(tokens), intValHex);
		case 4:
			let strVal = TryParseString(nextToken, next);
			return Expression_createStringConstant(Tokens_pop(tokens), strVal);
		case 2:
			if (Tokens_isSequenceNext2(tokens, null, "=>")) {
				return ParseLambda(tokens);
			}
			let varName = Tokens_popName(tokens, "variable name");
			return Expression_createVariable(varName, varName[0]);
	}
	Errors_Throw(nextToken, ["Expected an expression but found '", next, "' instead."].join(''));
	return null;
};

let ParseBitshift = function(tokens) {
	let root = ParseAddition(tokens);
	if (Tokens_doesNextInclulde3(tokens, "<<", ">>", ">>>")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_doesNextInclulde3(tokens, "<<", ">>", ">>>")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseAddition(tokens));
		}
		root = FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseBitwise = function(tokens) {
	let root = ParseEquality(tokens);
	if (Tokens_doesNextInclulde3(tokens, "&", "|", "^")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_doesNextInclulde3(tokens, "&", "|", "^")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseEquality(tokens));
		}
		return FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseBooleanCombination = function(tokens) {
	let root = ParseBitwise(tokens);
	if (Tokens_doesNextInclulde2(tokens, "||", "&&")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_doesNextInclulde2(tokens, "||", "&&")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseBitwise(tokens));
		}
		return FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseBreakContinue = function(tokens) {
	let expectedNextToken = "break";
	if (Tokens_isNext(tokens, "continue")) {
		expectedNextToken = "continue";
	}
	let token = Tokens_popKeyword(tokens, expectedNextToken);
	Tokens_popExpected(tokens, ";");
	return Statement_createBreakContinue(token);
};

let ParseClass = function(ctx, file, namespacePrefix) {
	let tokens = file[3];
	let classToken = Tokens_popKeyword(tokens, "class");
	let classNameToken = Tokens_popName(tokens, "class name");
	let baseClassTokens = null;
	if (Tokens_popIfPresent(tokens, ":")) {
		let errMsg = "base class or interface name";
		let parent = [];
		parent.push(Tokens_popName(tokens, errMsg));
		while (Tokens_isNext(tokens, ".")) {
			parent.push(Tokens_pop(tokens));
			parent.push(Tokens_popName(tokens, errMsg));
		}
		baseClassTokens = [...(parent)];
	}
	let classFqName = classNameToken[0];
	if (namespacePrefix != "") {
		classFqName = [namespacePrefix, ".", classNameToken[0]].join('');
	}
	let classDef = ClassEntity_new(classToken, classNameToken, classFqName);
	classDef[0] = baseClassTokens;
	Tokens_popExpected(tokens, "{");
	ParseOutEntities(ctx, file, classDef[5], classDef[6], classFqName);
	Tokens_popExpected(tokens, "}");
	if (!(classDef[5]["@ctor"] !== undefined)) {
		let baseArgs = null;
		if (classDef[0] != null) {
			baseArgs = [];
		}
		let ctor = FunctionEntity_BuildConstructor(classToken, [], [], baseArgs, [], false)[3];
		AttachEntityToParseTree(ctor, classDef[6], classDef[6][9], classDef[6][5], classDef[5], {});
	}
	return classDef;
};

let ParseCodeBlock = function(tokens, requireCurlyBrace) {
	return [...(ParseCodeBlockList(tokens, requireCurlyBrace))];
};

let ParseCodeBlockList = function(tokens, requireCurlyBrace) {
	let curlyBraceNext = requireCurlyBrace || Tokens_isNext(tokens, "{");
	let output = [];
	if (curlyBraceNext) {
		Tokens_popExpected(tokens, "{");
		while (!Tokens_popIfPresent(tokens, "}")) {
			output.push(ParseStatement(tokens, false));
		}
	} else {
		output.push(ParseStatement(tokens, false));
	}
	return output;
};

let ParseConst = function(tokens) {
	let constKeyword = Tokens_popKeyword(tokens, "const");
	let nameToken = Tokens_popName(tokens, "constant name");
	Tokens_popExpected(tokens, "=");
	let constValue = ParseExpression(tokens);
	Tokens_popExpected(tokens, ";");
	return ConstEntity_new(constKeyword, nameToken, constValue)[1];
};

let ParseConstructor = function(tokens, annotations) {
	let ctorKeyword = Tokens_popKeyword(tokens, "constructor");
	let args = [];
	let argValues = [];
	ParseArgDefinitionList(tokens, args, argValues);
	let baseArgs = null;
	if (Tokens_popIfPresent(tokens, ":")) {
		let baseKeyword = Tokens_popKeyword(tokens, "base");
		baseArgs = [];
		Tokens_popExpected(tokens, "(");
		while (!Tokens_popIfPresent(tokens, ")")) {
			if (baseArgs.length > 0) {
				Tokens_popExpected(tokens, ",");
			}
			baseArgs.push(ParseExpression(tokens));
		}
	}
	let code = ParseCodeBlockList(tokens, true);
	let ctor = FunctionEntity_BuildConstructor(ctorKeyword, args, argValues, baseArgs, code, annotations["static"] !== undefined)[3];
	ctor[6] = annotations;
	return ctor;
};

let ParseDictionaryDefinition = function(tokens) {
	let openDictionaryToken = Tokens_popExpected(tokens, "{");
	let keys = [];
	let values = [];
	let nextAllowed = true;
	while (nextAllowed && !Tokens_isNext(tokens, "}")) {
		keys.push(ParseExpression(tokens));
		Tokens_popExpected(tokens, ":");
		values.push(ParseExpression(tokens));
		nextAllowed = Tokens_popIfPresent(tokens, ",");
	}
	Tokens_popExpected(tokens, "}");
	return Expression_createDictionaryDefinition(openDictionaryToken, [...(keys)], [...(values)]);
};

let ParseDoWhileLoop = function(tokens) {
	let doToken = Tokens_popKeyword(tokens, "do");
	let code = ParseCodeBlock(tokens, false);
	let whileToken = Tokens_popKeyword(tokens, "while");
	Tokens_popExpected(tokens, "(");
	let condition = ParseExpression(tokens);
	Tokens_popExpected(tokens, ")");
	Tokens_popExpected(tokens, ";");
	return Statement_createDoWhile(doToken, code, whileToken, condition);
};

let ParseEnum = function(tokens) {
	let enumKeyword = Tokens_popKeyword(tokens, "enum");
	let nameToken = Tokens_popName(tokens, "enum name");
	Tokens_popExpected(tokens, "{");
	let nextAllowed = true;
	let names = [];
	let values = [];
	while (nextAllowed && !Tokens_isNext(tokens, "}")) {
		names.push(Tokens_popName(tokens, "enum member name"));
		let value = null;
		if (Tokens_popIfPresent(tokens, "=")) {
			value = ParseExpression(tokens);
		}
		values.push(value);
		nextAllowed = Tokens_popIfPresent(tokens, ",");
	}
	Tokens_popExpected(tokens, "}");
	return EnumEntity_new(enumKeyword, nameToken, [...(names)], [...(values)])[2];
};

let ParseEquality = function(tokens) {
	let root = ParseInequality(tokens);
	if (Tokens_doesNextInclulde2(tokens, "==", "!=")) {
		let op = Tokens_pop(tokens);
		let right = ParseInequality(tokens);
		return Expression_createBinaryOp(root, op, right);
	}
	return root;
};

let ParseExponent = function(tokens) {
	let root = ParseUnaryPrefix(tokens);
	if (Tokens_isNext(tokens, "**")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_isNext(tokens, "**")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseUnaryPrefix(tokens));
		}
		root = FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseExpression = function(tokens) {
	return ParseTernary(tokens);
};

let ParseField = function(tokens, annotations) {
	let fieldKeyword = Tokens_popKeyword(tokens, "field");
	let nameToken = Tokens_popName(tokens, "field name");
	let defaultValue = null;
	let equalToken = null;
	if (Tokens_isNext(tokens, "=")) {
		equalToken = Tokens_pop(tokens);
		defaultValue = ParseExpression(tokens);
	}
	Tokens_popExpected(tokens, ";");
	let entity = FieldEntity_new(fieldKeyword, nameToken, equalToken, defaultValue)[2];
	entity[6] = annotations;
	return entity;
};

let ParseForEachLoop = function(tokens) {
	let forToken = Tokens_popKeyword(tokens, "for");
	Tokens_popExpected(tokens, "(");
	let varToken = Tokens_popName(tokens, "for loop iteration variable name");
	Tokens_popExpected(tokens, ":");
	let listExpr = ParseExpression(tokens);
	Tokens_popExpected(tokens, ")");
	let code = ParseCodeBlock(tokens, false);
	return Statement_createForEachLoop(forToken, varToken, listExpr, code);
};

let ParseFunctionDefinition = function(tokens, annotations, optionalParentClass) {
	let functionKeyword = Tokens_popKeyword(tokens, "function");
	let nameToken = Tokens_popName(tokens, "function name");
	let isStatic = annotations["@static"] !== undefined;
	let args = [];
	let argValues = [];
	ParseArgDefinitionList(tokens, args, argValues);
	let code = ParseCodeBlockList(tokens, true);
	let entity = FunctionEntity_BuildMethodOrStandalone(functionKeyword, nameToken, args, argValues, code, isStatic, optionalParentClass)[3];
	entity[6] = annotations;
	return entity;
};

let ParseIfStatement = function(tokens) {
	let ifToken = Tokens_popKeyword(tokens, "if");
	Tokens_popExpected(tokens, "(");
	let condition = ParseExpression(tokens);
	Tokens_popExpected(tokens, ")");
	let ifCode = ParseCodeBlock(tokens, false);
	let elseCode = null;
	if (Tokens_popIfPresent(tokens, "else")) {
		elseCode = ParseCodeBlock(tokens, false);
	} else {
		elseCode = PST$createNewArray(0);
	}
	return Statement_createIfStatement(ifToken, condition, ifCode, elseCode);
};

let ParseInequality = function(tokens) {
	let root = ParseBitshift(tokens);
	if (Tokens_doesNextInclude5(tokens, "<", ">", "<=", ">=", "is")) {
		let op = Tokens_pop(tokens);
		let right = ParseBitshift(tokens);
		root = Expression_createBinaryOp(root, op, right);
	}
	return root;
};

let ParseInlineIncrementPrefix = function(tokens) {
	if (!Tokens_doesNextInclulde2(tokens, "++", "--")) {
		Tokens_popExpected(tokens, "++");
	}
	let op = Tokens_pop(tokens);
	let root = ParseUnarySuffix(tokens);
	return Expression_createInlineIncrement(op, root, op, true);
};

let ParseLambda = function(tokens) {
	let firstToken = Tokens_peek(tokens);
	let argTokens = [];
	let argDefaultValues = [];
	if (Tokens_popIfPresent(tokens, "(")) {
		while (!Tokens_popIfPresent(tokens, ")")) {
			if (argTokens.length > 0) {
				Tokens_popExpected(tokens, ",");
			}
			argTokens.push(Tokens_popName(tokens, "argument name"));
			let defaultVal = null;
			if (Tokens_popIfPresent(tokens, "=")) {
				defaultVal = ParseExpression(tokens);
			}
			argDefaultValues.push(defaultVal);
		}
	} else {
		argTokens.push(Tokens_popName(tokens, "argument name"));
		argDefaultValues.push(null);
	}
	let arrow = Tokens_popExpected(tokens, "=>");
	let code = null;
	if (Tokens_isNext(tokens, "{")) {
		code = ParseCodeBlock(tokens, true);
	} else {
		let codeExpr = ParseExpression(tokens);
		code = PST$createNewArray(1);
		code[0] = Statement_createReturn(arrow, codeExpr);
	}
	return Expression_createLambda(firstToken, [...(argTokens)], [...(argDefaultValues)], arrow, code);
};

let ParseListDefinition = function(tokens) {
	let openListToken = Tokens_popExpected(tokens, "[");
	let items = [];
	let nextAllowed = true;
	while (nextAllowed && !Tokens_isNext(tokens, "]")) {
		items.push(ParseExpression(tokens));
		nextAllowed = Tokens_popIfPresent(tokens, ",");
	}
	Tokens_popExpected(tokens, "]");
	return Expression_createListDefinition(openListToken, [...(items)]);
};

let ParseMultiplication = function(tokens) {
	let root = ParseExponent(tokens);
	if (Tokens_doesNextInclulde3(tokens, "*", "/", "%")) {
		let expressions = [];
		expressions.push(root);
		let ops = [];
		while (Tokens_doesNextInclulde3(tokens, "*", "/", "%")) {
			ops.push(Tokens_pop(tokens));
			expressions.push(ParseExponent(tokens));
		}
		root = FlattenBinaryOpChain(expressions, ops);
	}
	return root;
};

let ParseNamespace = function(ctx, file, namespacePrefix) {
	let tokens = file[3];
	let nsToken = Tokens_popKeyword(tokens, "namespace");
	let namespaceChain = [];
	if (namespacePrefix != "") {
		namespaceChain.push(namespacePrefix);
	}
	let nsFirst = Tokens_popName(tokens, "namespace name");
	namespaceChain.push(nsFirst[0]);
	let entityBucket = {};
	while (Tokens_popIfPresent(tokens, ".")) {
		fail("Not implemented");
	}
	let nsEntity = NamespaceEntity_new(nsToken, nsFirst, namespacePrefix);
	Tokens_popExpected(tokens, "{");
	namespacePrefix = namespaceChain.join(".");
	ParseOutEntities(ctx, file, nsEntity[0], nsEntity[1], namespacePrefix);
	Tokens_popExpected(tokens, "}");
	return nsEntity[1];
};

let ParseNegatePrefix = function(tokens) {
	if (!Tokens_doesNextInclulde3(tokens, "-", "!", "~")) {
		Tokens_popExpected(tokens, "-");
	}
	let op = Tokens_pop(tokens);
	let root = ParseUnaryPrefix(tokens);
	return Expression_createNegatePrefix(op, root);
};

let ParseNullCoalesce = function(tokens) {
	let root = ParseBooleanCombination(tokens);
	if (Tokens_isNext(tokens, "??")) {
		let op = Tokens_pop(tokens);
		let next = ParseNullCoalesce(tokens);
		root = Expression_createBinaryOp(root, op, next);
	}
	return root;
};

let ParseOutEntities = function(compiler, file, currentEntityBucket, nestParent, namespacePrefix) {
	let tokens = file[3];
	let keepChecking = Tokens_hasMore(tokens);
	let wrappingClass = null;
	if (nestParent != null && nestParent[1] == 1) {
		wrappingClass = nestParent[2];
	}
	while (keepChecking) {
		let firstToken = Tokens_peek(tokens);
		let annotationTokens = ParseAnnotations(compiler, tokens);
		let nextToken = Tokens_peekValueNonNull(tokens);
		let entity = null;
		switch (EntityParser_ClassifyToken(nextToken)) {
			case 1:
				entity = ParseFunctionDefinition(tokens, annotationTokens, wrappingClass);
				break;
			case 2:
				entity = ParseNamespace(compiler, file, namespacePrefix);
				break;
			case 3:
				entity = ParseConst(tokens);
				break;
			case 4:
				entity = ParseEnum(tokens);
				break;
			case 5:
				entity = ParseClass(compiler, file, namespacePrefix)[6];
				break;
			case 6:
				entity = ParseConstructor(tokens, annotationTokens);
				break;
			case 7:
				entity = ParseField(tokens, annotationTokens);
				break;
			case 8:
				fail("Not implemented");
				break;
			case 9:
				Errors_Throw(Tokens_peek(tokens), "All imports must appear at the top of the file.");
				break;
			case 10:
				keepChecking = false;
				break;
			default:
				Tokens_ensureMore(tokens);
				Errors_Throw(Tokens_peek(tokens), ["Unexpected token: '", Tokens_peekValueNonNull(tokens), "'"].join(''));
				break;
		}
		if (entity != null) {
			entity[7] = entity[6] != null && entity[6]["static"] !== undefined;
			AttachEntityToParseTree(entity, nestParent, file, namespacePrefix, currentEntityBucket, annotationTokens);
		}
		if (entity == null && Object.keys(annotationTokens).length > 0) {
			Errors_Throw(firstToken, "This annotation is not attached to any entity.");
		}
		if (!Tokens_hasMore(tokens)) {
			keepChecking = false;
		}
	}
};

let ParseReturn = function(tokens) {
	let retToken = Tokens_popKeyword(tokens, "return");
	let expr = null;
	if (!Tokens_isNext(tokens, ";")) {
		expr = ParseExpression(tokens);
	} else {
		expr = Expression_createNullConstant(null);
	}
	Tokens_popExpected(tokens, ";");
	return Statement_createReturn(retToken, expr);
};

let ParseStatement = function(tokens, isForLoop) {
	let nextToken = Tokens_peek(tokens);
	if (!isForLoop && nextToken != null && nextToken[2] == 1) {
		switch (StatementParser_IdentifyKeywordType(nextToken[0])) {
			case 1:
				return ParseBreakContinue(tokens);
			case 2:
				return ParseBreakContinue(tokens);
			case 3:
				return ParseDoWhileLoop(tokens);
			case 4:
				return ParseAnyForLoop(tokens);
			case 5:
				return ParseIfStatement(tokens);
			case 6:
				return ParseReturn(tokens);
			case 7:
				return ParseSwitch(tokens);
			case 8:
				return ParseThrow(tokens);
			case 9:
				return ParseTry(tokens);
			case 10:
				return ParseWhileLoop(tokens);
			case 11:
				fail("Not implemented");
				return null;
			default:
				fail("");
				break;
		}
	}
	let expr = ParseExpression(tokens);
	let assignOp = TryPopAssignmentOp(tokens);
	let s = null;
	if (assignOp != null) {
		let assignValue = ParseExpression(tokens);
		s = Statement_createAssignment(expr, assignOp, assignValue);
	} else {
		s = Statement_createExpressionAsStatement(expr);
	}
	if (!isForLoop) {
		Tokens_popExpected(tokens, ";");
	}
	return s;
};

let ParseSwitch = function(tokens) {
	let switchToken = Tokens_popKeyword(tokens, "switch");
	Tokens_popExpected(tokens, "(");
	let condition = ParseExpression(tokens);
	Tokens_popExpected(tokens, ")");
	let chunks = [];
	Tokens_popExpected(tokens, "{");
	let defaultEncountered = false;
	while (!Tokens_popIfPresent(tokens, "}")) {
		Tokens_ensureMore(tokens);
		let activeChunk = SwitchChunk_new();
		chunks.push(activeChunk);
		while (Tokens_isNext(tokens, "case") || Tokens_isNext(tokens, "default")) {
			if (defaultEncountered) {
				Errors_Throw(Tokens_peek(tokens), "The default case for a switch statement must appear at the end.");
			}
			if (Tokens_isNext(tokens, "case")) {
				let caseToken = Tokens_popKeyword(tokens, "case");
				let caseValue = ParseExpression(tokens);
				activeChunk[0].push(caseToken);
				activeChunk[1].push(caseValue);
			} else {
				let defaultToken = Tokens_popKeyword(tokens, "default");
				activeChunk[0].push(defaultToken);
				activeChunk[1].push(null);
				defaultEncountered = true;
			}
			Tokens_popExpected(tokens, ":");
		}
		while (!Tokens_isNext(tokens, "case") && !Tokens_isNext(tokens, "default") && !Tokens_isNext(tokens, "}") && Tokens_hasMore(tokens)) {
			activeChunk[2].push(ParseStatement(tokens, false));
		}
	}
	return Statement_createSwitchStatement(switchToken, condition, [...(chunks)]);
};

let ParseTernary = function(tokens) {
	let root = ParseNullCoalesce(tokens);
	if (Tokens_isNext(tokens, "?")) {
		let qmark = Tokens_pop(tokens);
		let trueValue = ParseTernary(tokens);
		Tokens_popExpected(tokens, ":");
		let falseValue = ParseTernary(tokens);
		root = Expression_createTernary(root, qmark, trueValue, falseValue);
	}
	return root;
};

let ParseThrow = function(tokens) {
	let throwToken = Tokens_popKeyword(tokens, "throw");
	let value = ParseExpression(tokens);
	Tokens_popExpected(tokens, ";");
	return Statement_createThrow(throwToken, value);
};

let ParseTraditionalForLoop = function(tokens) {
	let forToken = Tokens_popKeyword(tokens, "for");
	Tokens_popExpected(tokens, "(");
	let init = [];
	let condition = null;
	let step = [];
	if (!Tokens_isNext(tokens, ";")) {
		init.push(ParseStatement(tokens, true));
		while (Tokens_popIfPresent(tokens, ",")) {
			init.push(ParseStatement(tokens, true));
		}
	}
	Tokens_popExpected(tokens, ";");
	if (!Tokens_isNext(tokens, ";")) {
		condition = ParseExpression(tokens);
	}
	Tokens_popExpected(tokens, ";");
	if (!Tokens_isNext(tokens, ")")) {
		step.push(ParseStatement(tokens, true));
		while (Tokens_popIfPresent(tokens, ",")) {
			step.push(ParseStatement(tokens, true));
		}
	}
	Tokens_popExpected(tokens, ")");
	let code = ParseCodeBlock(tokens, false);
	return Statement_createForLoop(forToken, [...(init)], condition, [...(step)], code);
};

let ParseTry = function(tokens) {
	let tryToken = Tokens_popKeyword(tokens, "try");
	let tryCode = ParseCodeBlock(tokens, true);
	let catches = [];
	let finallyToken = null;
	let finallyCode = null;
	while (Tokens_isNext(tokens, "catch")) {
		Tokens_popKeyword(tokens, "catch");
		let classNamesRaw = [];
		let exceptionVarToken = null;
		Tokens_popExpected(tokens, "(");
		let mysteryToken = Tokens_popName(tokens, "exception name or variable");
		Tokens_ensureMore(tokens);
		if (Tokens_popIfPresent(tokens, ")")) {
			exceptionVarToken = mysteryToken;
		} else {
			let classNameBuilder = [];
			classNameBuilder.push(mysteryToken);
			while (Tokens_popIfPresent(tokens, ".")) {
				classNameBuilder.push(Tokens_popName(tokens, "exception name"));
			}
			classNamesRaw.push([...(classNameBuilder)]);
			while (Tokens_popIfPresent(tokens, "|")) {
				PST$clearList(classNameBuilder);
				classNameBuilder.push(Tokens_popName(tokens, "exception name"));
				while (Tokens_popIfPresent(tokens, ".")) {
					classNameBuilder.push(Tokens_popName(tokens, "exception name"));
				}
				classNamesRaw.push([...(classNameBuilder)]);
			}
			if (!Tokens_isNext(tokens, ")")) {
				exceptionVarToken = Tokens_popName(tokens, "exception variable name");
			}
			Tokens_popExpected(tokens, ")");
		}
		let catchCode = ParseCodeBlock(tokens, true);
		catches.push(CatchChunk_new(catchCode, classNamesRaw, exceptionVarToken));
	}
	if (Tokens_isNext(tokens, "finally")) {
		finallyToken = Tokens_popKeyword(tokens, "finally");
		finallyCode = ParseCodeBlock(tokens, true);
	} else {
		finallyCode = PST$createNewArray(0);
	}
	return Statement_createTry(tryToken, tryCode, [...(catches)], finallyToken, finallyCode);
};

let ParseTypeofPrefix = function(tokens) {
	let typeofToken = Tokens_popKeyword(tokens, "typeof");
	let root = ParseUnaryPrefix(tokens);
	return Expression_createTypeof(typeofToken, root);
};

let ParseUnaryPrefix = function(tokens) {
	let next = Tokens_peekValue(tokens);
	if (next == null) {
		Tokens_ensureMore(tokens);
	}
	switch (next.charAt(0)) {
		case "-":
			if (next == "--") {
				return ParseInlineIncrementPrefix(tokens);
			}
			if (next == "-") {
				return ParseNegatePrefix(tokens);
			}
			break;
		case "+":
			if (next == "++") {
				return ParseInlineIncrementPrefix(tokens);
			}
			break;
		case "!":
			if (next == "!") {
				return ParseNegatePrefix(tokens);
			}
			break;
		case "~":
			if (next == "~") {
				return ParseNegatePrefix(tokens);
			}
			break;
		case "t":
			if (next == "typeof") {
				return ParseTypeofPrefix(tokens);
			}
			break;
	}
	return ParseUnarySuffix(tokens);
};

let ParseUnarySuffix = function(tokens) {
	let root = ParseAtomicExpression(tokens);
	let next = Tokens_peekValue(tokens);
	let checkForSuffixes = true;
	while (checkForSuffixes && next != null) {
		checkForSuffixes = false;
		if (next == ".") {
			let dotToken = Tokens_pop(tokens);
			let nameToken = Tokens_popName(tokens, "field name");
			root = Expression_createDotField(root, dotToken, nameToken[0]);
			checkForSuffixes = true;
		} else if (next == "(") {
			let openParen = Tokens_pop(tokens);
			let args = [];
			while (!Tokens_popIfPresent(tokens, ")")) {
				if (args.length > 0) {
					Tokens_popExpected(tokens, ",");
				}
				args.push(ParseExpression(tokens));
			}
			root = Expression_createFunctionInvocation(root, openParen, [...(args)]);
			checkForSuffixes = true;
		} else if (next == "[") {
			let openBracket = Tokens_pop(tokens);
			Tokens_ensureMore(tokens);
			let throwTokenOnInvalidSlice = Tokens_peek(tokens);
			let sliceNums = [];
			let nextExpected = true;
			while (nextExpected && Tokens_hasMore(tokens)) {
				if (Tokens_popIfPresent(tokens, ":")) {
					sliceNums.push(null);
				} else if (Tokens_isNext(tokens, "]")) {
					sliceNums.push(null);
					nextExpected = false;
				} else {
					sliceNums.push(ParseExpression(tokens));
					if (!Tokens_popIfPresent(tokens, ":")) {
						nextExpected = false;
					}
				}
			}
			Tokens_popExpected(tokens, "]");
			if (sliceNums.length < 0 || sliceNums.length > 3) {
				Errors_Throw(throwTokenOnInvalidSlice, "Invalid index or slice expression");
			}
			if (sliceNums.length == 1) {
				if (sliceNums[0] == null) {
					Errors_Throw(throwTokenOnInvalidSlice, "Expected index expression.");
				}
				root = Expression_createBracketIndex(root, openBracket, sliceNums[0]);
			} else {
				let thirdSliceNum = null;
				if (sliceNums.length == 3) {
					thirdSliceNum = sliceNums[2];
				}
				root = Expression_createSliceExpression(root, openBracket, sliceNums[0], sliceNums[1], thirdSliceNum);
			}
			checkForSuffixes = true;
		} else if (next == "++" || next == "--") {
			let ppToken = Tokens_pop(tokens);
			root = Expression_createInlineIncrement(root[0], root, ppToken, false);
		}
		next = Tokens_peekValue(tokens);
	}
	return root;
};

let ParseWhileLoop = function(tokens) {
	let whileToken = Tokens_popKeyword(tokens, "while");
	Tokens_popExpected(tokens, "(");
	let condition = ParseExpression(tokens);
	Tokens_popExpected(tokens, ")");
	let code = ParseCodeBlock(tokens, false);
	return Statement_createWhileLoop(whileToken, condition, code);
};

let PerformFullResolutionPassOnConstAndEnums = function(resolver, resOrder) {
	let passNum = 1;
	while (passNum <= 2) {
		let i = 0;
		while (i < resOrder.length) {
			let entity = resolver[4][resOrder[i]];
			resolver[7] = entity;
			if (entity[1] == 4) {
				let enumDef = entity[2];
				let memberIndex = Resolver_GetEnumMemberIndex(resolver, resOrder[i], enumDef);
				let val = enumDef[1][memberIndex];
				if (passNum == 1) {
					val = ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
				} else {
					val = ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
				}
				if (passNum == 2 && val[1] != 22) {
					Errors_Throw(enumDef[0][memberIndex], "This enum value has a non-integer value.");
				}
				enumDef[1][memberIndex] = val;
			} else {
				let constEnt = entity[2];
				let val = constEnt[0];
				if (passNum == 1) {
					val = ExpressionResolver_ResolveExpressionFirstPass(resolver, val);
				} else {
					val = ExpressionResolver_ResolveExpressionSecondPass(resolver, val);
					if (!IsExpressionConstant(val)) {
						Errors_Throw(val[0], "A constant expression is required here.");
					}
				}
				constEnt[0] = val;
			}
			resolver[7] = null;
			i += 1;
		}
		passNum += 1;
	}
};

let PUBLIC_CompleteCompilation = function(compObj) {
	let compiler = compObj;
	let moduleCompilationOrder = CompilerContext_CalculateCompilationOrder(compiler);
	compiler[6] = {};
	let i = 0;
	while (i < moduleCompilationOrder.length) {
		let moduleId = moduleCompilationOrder[i];
		let module = CompilerContext_CompileModule(compiler, moduleId);
		compiler[6][moduleId] = module;
		i += 1;
	}
	let modules = Object.values(compiler[6]);
	let bundle = bundleCompilation(compiler[0], compiler[1], modules);
	return ExportUtil_exportBundle(compiler[8], compiler[7], bundle);
};

let PUBLIC_EnsureDependenciesFulfilled = function(compObj) {
	let compiler = compObj;
	if (Object.keys(compiler[5]).length > 0) {
		fail("Not all dependencies are fulfilled.");
	}
};

let PUBLIC_GetNextRequiredModuleId = function(compObj) {
	let compiler = compObj;
	while (true) {
		if (Object.keys(compiler[5]).length == 0) {
			return null;
		}
		let unfulfilledDependencies = PST$sortedCopyOfArray(Object.keys(compiler[5]));
		let nextKey = unfulfilledDependencies[0];
		if (!IsBuiltInModule(nextKey)) {
			return nextKey;
		}
		let builtinFiles = {};
		builtinFiles[nextKey + ".script"] = GetSourceForBuiltinModule(nextKey);
		PUBLIC_SupplyFilesForModule(compiler, nextKey, builtinFiles, false, true);
	}
};

let PUBLIC_getTokenErrPrefix = function(tok) {
	if (tok == null) {
		return "";
	}
	return ["[", tok[1], " Line ", tok[3] + '', " Col ", tok[4] + '', "] "].join('');
};

let PUBLIC_SupplyFilesForModule = function(compObj, moduleId, fileLookup, isCoreBuiltin, isBuiltInLib) {
	let i = 0;
	let j = 0;
	let compiler = compObj;
	compiler[2][moduleId] = [];
	let files = [];
	let imports = {};
	let fileNamesOrdered = PST$sortedCopyOfArray(Object.keys(fileLookup));
	i = 0;
	while (i < fileNamesOrdered.length) {
		let path = fileNamesOrdered[i];
		let fileCtx = FileContext_new(compiler[0], path, fileLookup[path]);
		fileCtx[7] = isCoreBuiltin;
		fileCtx[6] = isBuiltInLib;
		files.push(fileCtx);
		fileCtx[4] = ImportParser_AdvanceThroughImports(fileCtx[3], isCoreBuiltin);
		FileContext_initializeImportLookup(fileCtx);
		j = 0;
		while (j < fileCtx[4].length) {
			let impStmnt = fileCtx[4][j];
			imports[impStmnt[2]] = impStmnt;
			j += 1;
		}
		i += 1;
	}
	compiler[3][moduleId] = files;
	if (compiler[5][moduleId] !== undefined) {
		delete compiler[5][moduleId];
	}
	let allDeps = [];
	let importedIds = Object.keys(imports);
	i = 0;
	while (i < importedIds.length) {
		allDeps.push(importedIds[i]);
		i += 1;
	}
	compiler[2][moduleId] = allDeps;
	i = 0;
	while (i < allDeps.length) {
		let depId = allDeps[i];
		if (!(compiler[3][depId] !== undefined)) {
			compiler[5][depId] = true;
		}
		i += 1;
	}
};

let Resolve = function(resolver) {
	let i = 0;
	let functions = [];
	let classes = [];
	let constants = [];
	let enums = [];
	let constructors = [];
	let fields = [];
	let entities = Object.values(resolver[3]);
	i = 0;
	while (i < entities.length) {
		let tle = entities[i];
		if (tle[1] == 2) {
			constants.push(tle[2]);
		} else if (tle[1] == 4) {
			enums.push(tle[2]);
		} else if (tle[1] == 6) {
			functions.push(tle[2]);
		} else if (tle[1] == 1) {
			classes.push(tle[2]);
		} else if (tle[1] == 3) {
			constructors.push(tle[2]);
		} else if (tle[1] == 5) {
			fields.push(tle[2]);
		} else if (tle[1] == 7) {
		} else {
			fail("Not implemented");
		}
		i += 1;
	}
	AddImplicitIncrementingEnumValueDefinitions(enums);
	let constAndEnumResolutionOrder = Resolver_DetermineConstAndEnumResolutionOrder(resolver, constants, enums);
	PerformFullResolutionPassOnConstAndEnums(resolver, constAndEnumResolutionOrder);
	let orderedClasses = ResolveBaseClassesAndEstablishClassOrder(resolver, classes, resolver[3]);
	i = 0;
	while (i < orderedClasses.length) {
		EntityResolver_DetermineMemberOffsets(orderedClasses[i]);
		i += 1;
	}
	i = 0;
	while (i < functions.length) {
		EntityResolver_ResetAutoVarId(resolver);
		EntityResolver_ResolveFunctionFirstPass(resolver, functions[i]);
		i += 1;
	}
	i = 0;
	while (i < constructors.length) {
		EntityResolver_ResetAutoVarId(resolver);
		EntityResolver_ResolveFunctionFirstPass(resolver, constructors[i]);
		i += 1;
	}
	i = 0;
	while (i < resolver[6].length) {
		EntityResolver_ResetAutoVarId(resolver);
		EntityResolver_ResolveFunctionFirstPass(resolver, resolver[6][i]);
		i += 1;
	}
	i = 0;
	while (i < functions.length) {
		EntityResolver_ResolveFunctionSecondPass(resolver, functions[i]);
		i += 1;
	}
	i = 0;
	while (i < constructors.length) {
		EntityResolver_ResolveFunctionSecondPass(resolver, constructors[i]);
		i += 1;
	}
	i = 0;
	while (i < resolver[6].length) {
		EntityResolver_ResolveFunctionSecondPass(resolver, resolver[6][i]);
		i += 1;
	}
};

let ResolveBaseClassesAndEstablishClassOrder = function(resolver, classes, flattenedEntities) {
	let i = 0;
	let j = 0;
	let deterministicOrder = ClassSorter_SortClassesInDeterministicDependencyOrder([...(classes)], false);
	let finalOrder = [];
	let baseClassRequired = [];
	i = 0;
	while (i < deterministicOrder.length) {
		let e = deterministicOrder[i];
		if (e[0] != null) {
			baseClassRequired.push(e);
		} else {
			finalOrder.push(e);
		}
		i += 1;
	}
	i = 0;
	while (i < baseClassRequired.length) {
		let bc = baseClassRequired[i];
		resolver[7] = bc[6];
		let baseClassToken = bc[0][0];
		let bcEntity = LookupUtil_DoLookupForName(resolver, baseClassToken, baseClassToken[0]);
		if (bcEntity != null) {
			j = 2;
			while (j < bc[0].length) {
				let next = bc[0][j][0];
				if (bcEntity != null) {
					let lookup = Entity_getMemberLookup(resolver[0], bcEntity);
					if (lookup[next] !== undefined) {
						bcEntity = lookup[next];
					} else {
						bcEntity = null;
					}
				}
				j += 2;
			}
		}
		if (bcEntity == null) {
			Errors_Throw(bc[6][0], "Could not resolve base class");
			fail("Not implemented");
		}
		if (bcEntity[1] != 1) {
			Errors_Throw(bc[6][0], bcEntity[5] + " is not a valid class.");
		}
		bc[1] = bcEntity[2];
		resolver[7] = null;
		i += 1;
	}
	let includedInOrder = {};
	i = 0;
	while (i < baseClassRequired.length) {
		let bc = baseClassRequired[i];
		includedInOrder[bc[6][5]] = false;
		i += 1;
	}
	i = 0;
	while (i < baseClassRequired.length) {
		let bc = baseClassRequired[i];
		let walker = bc;
		let order = [];
		while (walker != null) {
			if (!(includedInOrder[walker[6][5]] !== undefined) || includedInOrder[walker[6][5]]) {
				walker = null;
			} else {
				order.push(walker);
				includedInOrder[walker[6][5]] = true;
				walker = walker[1];
			}
			if (order.length > deterministicOrder.length) {
				Errors_Throw(bc[6][0], "This class has a cycle in its base class chain.");
			}
		}
		order.reverse();
		j = 0;
		while (j < order.length) {
			finalOrder.push(order[j]);
			j += 1;
		}
		i += 1;
	}
	return [...(finalOrder)];
};

let Resolver_DetermineConstAndEnumResolutionOrder = function(resolver, constants, enums) {
	let i = 0;
	let referencesMadeByFqItem = {};
	i = 0;
	while (i < constants.length) {
		let c = constants[i];
		let ns = "";
		if (c[1][8] != null) {
			ns = c[1][8][5];
		}
		let refsOut = [];
		c[0] = Resolver_GetListOfUnresolvedConstReferences(resolver, c[1][9], ns, c[0], refsOut);
		referencesMadeByFqItem[c[1][5]] = refsOut;
		i += 1;
	}
	i = 0;
	while (i < enums.length) {
		let e = enums[i];
		let ns = "";
		if (e[2][8] != null) {
			ns = e[2][8][5];
		}
		let memCount = e[0].length;
		let j = 0;
		while (j < memCount) {
			let memFqName = [e[2][5], ".", e[0][j][0]].join('');
			let val = e[1][j];
			let refsOut = [];
			e[1][j] = Resolver_GetListOfUnresolvedConstReferences(resolver, e[2][9], ns, val, refsOut);
			referencesMadeByFqItem[memFqName] = refsOut;
			j++;
		}
		i += 1;
	}
	let resolutionStatus = {};
	let resolutionOrder = [];
	let q = StringArrayToList(PST$sortedCopyOfArray(Object.keys(referencesMadeByFqItem)));
	q.reverse();
	while (q.length > 0) {
		let lastIndex = q.length - 1;
		let itemFqName = q[lastIndex];
		q.splice(lastIndex, 1);
		if (itemFqName == "*") {
			let resolvedItem = q[lastIndex - 1];
			q.splice(lastIndex - 1, 1);
			resolutionStatus[resolvedItem] = 2;
			resolutionOrder.push(resolvedItem);
		} else {
			let item = resolver[4][itemFqName];
			let itemToken = item[0];
			if (item[5] != itemFqName) {
				let parentEnum = item[2];
				let enumValIndex = Resolver_GetEnumMemberIndex(resolver, itemFqName, parentEnum);
				itemToken = parentEnum[0][enumValIndex];
			}
			let status = 0;
			if (resolutionStatus[itemFqName] !== undefined) {
				status = resolutionStatus[itemFqName];
			}
			if (status == 1) {
				Errors_Throw(item[0], "This definition contains a resolution cycle.");
			}
			if (status == 2) {
			} else {
				q.push(itemFqName);
				q.push("*");
				resolutionStatus[itemFqName] = 1;
				let references = referencesMadeByFqItem[itemFqName];
				i = references.length - 1;
				while (i >= 0) {
					q.push(references[i]);
					i -= 1;
				}
			}
		}
	}
	return [...(resolutionOrder)];
};

let Resolver_GetEnumMemberIndex = function(resolver, memNameOrFqMemName, enumEntity) {
	let lastDot = memNameOrFqMemName.lastIndexOf(".");
	let memName = memNameOrFqMemName;
	if (lastDot != -1) {
		memName = memNameOrFqMemName.substring(lastDot + 1, lastDot + 1 + (memNameOrFqMemName.length - lastDot - 1));
	}
	let i = 0;
	while (i < enumEntity[0].length) {
		if (enumEntity[0][i][0] == memName) {
			return i;
		}
		i += 1;
	}
	return -1;
};

let Resolver_GetListOfUnresolvedConstReferences = function(resolver, file, fqNamespace, expr, refsOut) {
	return Resolver_GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr, refsOut);
};

let Resolver_GetListOfUnresolvedConstReferencesImpl = function(resolver, file, fqNamespace, expr, refs) {
	switch (expr[1]) {
		case 22:
			return expr;
		case 5:
			return expr;
		case 16:
			return expr;
		case 26:
			return expr;
		case 28:
			return expr;
		case 3:
			expr[3] = Resolver_GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr[3], refs);
			expr[4] = Resolver_GetListOfUnresolvedConstReferencesImpl(resolver, file, fqNamespace, expr[4], refs);
			return expr;
		case 31:
			let referenced = TryDoExactLookupForConstantEntity(resolver, file, fqNamespace, expr[7]);
			if (referenced == null) {
				Errors_Throw(expr[0], ["No definition for '", expr[7], "'"].join(''));
			} else {
				if (referenced[9][8] != file[8]) {
					if (referenced[1] == 2) {
						fail("Not implemented");
					} else if (referenced[1] == 4) {
						fail("Not implemented");
					} else {
						fail("Not implemented");
					}
				}
				switch (referenced[1]) {
					case 2:
						refs.push(referenced[5]);
						break;
					case 4:
						fail("Not implemented");
						break;
					default:
						Errors_Throw(expr[0], "Cannot refer to this entity from a constant expression.");
						break;
				}
			}
			return expr;
		case 11:
			let fullRefSegments = DotField_getVariableRootedDottedChain(expr, "Cannot use this type of entity from a constant expression.");
			let fullRefDotted = fullRefSegments.join(".");
			let reffedEntity = TryDoExactLookupForConstantEntity(resolver, file, fqNamespace, fullRefDotted);
			if (reffedEntity == null) {
				Errors_Throw(expr[0], "Invalid expression for constant.");
			}
			if (reffedEntity[9][8] != file[8]) {
				if (reffedEntity[1] == 2) {
					return reffedEntity[2][0];
				}
				if (reffedEntity[1] == 4) {
					fail("Not implemented");
				} else {
					fail("Not implemented");
				}
			}
			if (reffedEntity[1] == 2) {
				refs.push(reffedEntity[5]);
			} else if (reffedEntity[1] == 4) {
				let enumMemberName = fullRefSegments[fullRefSegments.length - 1];
				let enumName = fullRefDotted.substring(0, 0 + (fullRefDotted.length - enumMemberName.length - 1));
				refs.push([reffedEntity[5], ".", enumMemberName].join(''));
			} else {
				Errors_Throw(expr[0], "Cannot reference this entity from here.");
			}
			return expr;
		default:
			Errors_Throw(expr[0], "Invalid expression for constant.");
			break;
	}
	return null;
};

let Resolver_isValidRegisteredExtension = function(resolver, extensionName) {
	return StringSet_has(resolver[11], extensionName);
};

let Resolver_new = function(staticCtx, rootEntities, extensionNames) {
	let r = [staticCtx, rootEntities, {}, {}, {}, {}, [], null, null, null, 0, StringSet_fromList(extensionNames)];
	r[8] = FlattenEntities(staticCtx, rootEntities);
	let i = 0;
	while (i < r[8].length) {
		let tle = r[8][i];
		r[3][tle[5]] = tle;
		r[4][tle[5]] = tle;
		if (tle[1] == 4) {
			let enumDef = tle[2];
			let j = 0;
			while (j < enumDef[0].length) {
				let fqName = [enumDef[2][5], ".", enumDef[0][j][0]].join('');
				r[2][fqName] = enumDef[2];
				r[4][fqName] = enumDef[2];
				r[5][fqName] = enumDef[2];
				j++;
			}
		} else {
			r[5][tle[5]] = tle;
		}
		i += 1;
	}
	return r;
};

let Resolver_ReportNewLambda = function(resolver, lamb) {
	resolver[6].push(lamb);
};

let serializeAssignField = function(staticCtx, assignField, baseOp) {
	let df = assignField[4];
	let fieldName = df[7];
	let bufVal = serializeExpression(staticCtx, assignField[5]);
	let bufRoot = serializeExpression(staticCtx, df[2]);
	if (baseOp != "=") {
		let incrBuf = join2(bufRoot, create0(52, null, null));
		incrBuf = join2(incrBuf, create0(15, df[5], fieldName));
		incrBuf = join3(incrBuf, bufVal, create0(4, assignField[6], baseOp));
		return join2(incrBuf, create1(1, assignField[6], fieldName, 1));
	}
	return join3(bufVal, bufRoot, create0(1, assignField[6], fieldName));
};

let serializeAssignIndex = function(staticCtx, assignIndex, baseOp) {
	let bracketToken = assignIndex[4][5];
	let bufVal = serializeExpression(staticCtx, assignIndex[5]);
	let bufIndex = serializeExpression(staticCtx, assignIndex[4][4]);
	let bufRoot = serializeExpression(staticCtx, assignIndex[4][2]);
	if (baseOp != "=") {
		let incrBuf = join3(bufRoot, bufIndex, create0(53, null, null));
		incrBuf = join2(incrBuf, create0(22, bracketToken, null));
		incrBuf = join3(incrBuf, bufVal, create0(4, assignIndex[6], baseOp));
		return join2(incrBuf, create1(2, assignIndex[6], null, 1));
	}
	return join4(bufVal, bufRoot, bufIndex, create0(2, assignIndex[6], null));
};

let serializeAssignVariable = function(staticCtx, assignVar, baseOp) {
	let bufVal = serializeExpression(staticCtx, assignVar[5]);
	let bufVar = serializeExpression(staticCtx, assignVar[4]);
	if (baseOp != "=") {
		bufVal = join3(bufVar, bufVal, create0(4, assignVar[6], baseOp));
	}
	return join2(bufVal, create0(3, assignVar[6], assignVar[4][7]));
};

let serializeBaseCtorReference = function(baseCtor) {
	let baseClass = baseCtor[10];
	return create1(35, baseCtor[0], null, baseClass[10]);
};

let serializeBinaryOp = function(staticCtx, binOp) {
	let opToken = binOp[5];
	let op = opToken[0];
	let left = binOp[3];
	let right = binOp[4];
	let leftBuf = serializeExpression(staticCtx, left);
	let rightBuf = serializeExpression(staticCtx, right);
	if (op == "??") {
		return join3(leftBuf, create1(31, opToken, null, rightBuf[0]), rightBuf);
	}
	if (op == "&&" || op == "||") {
		leftBuf = ByteCodeUtil_ensureBooleanExpression(left[0], leftBuf);
		rightBuf = ByteCodeUtil_ensureBooleanExpression(right[0], rightBuf);
		let opCode = 30;
		if (op == "&&") {
			opCode = 32;
		}
		return join3(leftBuf, create1(opCode, null, null, rightBuf[0]), rightBuf);
	}
	return join3(leftBuf, rightBuf, create0(4, opToken, op));
};

let serializeBitwiseNot = function(staticCtx, bwn) {
	return join2(ByteCodeUtil_ensureIntegerExpression(bwn[2][0], serializeExpression(staticCtx, bwn[2])), create0(10, bwn[0], null));
};

let serializeBoolConst = function(bc) {
	let boolVal = 0;
	if (bc[6]) {
		boolVal = 1;
	}
	return create1(36, bc[0], null, boolVal);
};

let serializeBooleanNot = function(staticCtx, bn) {
	return join2(serializeExpression(staticCtx, bn[2]), create0(11, bn[0], null));
};

let serializeBreak = function(br) {
	return create0(-1, br[0], null);
};

let serializeClassReference = function(classRef) {
	return create1(37, classRef[0], null, classRef[10][10]);
};

let serializeCodeBlock = function(staticCtx, block) {
	let buf = null;
	let i = 0;
	while (i < block.length) {
		buf = join2(buf, serializeStatement(staticCtx, block[i]));
		i += 1;
	}
	return buf;
};

let serializeConstructorInvocation = function(staticCtx, ctorInvoke) {
	let classDef = ctorInvoke[10];
	let buf = null;
	let argc = ctorInvoke[12].length;
	let i = 0;
	while (i < argc) {
		buf = join2(buf, serializeExpression(staticCtx, ctorInvoke[12][i]));
		i += 1;
	}
	return join3(create1(14, ctorInvoke[0], null, classDef[10]), buf, create1(21, ctorInvoke[5], null, argc));
};

let serializeContinue = function(cont) {
	return create0(-2, cont[0], null);
};

let serializeDictionaryDefinition = function(staticCtx, dictDef) {
	let sz = dictDef[13].length;
	let buf = null;
	let i = 0;
	while (i < sz) {
		buf = join3(buf, serializeExpression(staticCtx, dictDef[13][i]), serializeExpression(staticCtx, dictDef[14][i]));
		i += 1;
	}
	return join2(buf, create1(13, dictDef[0], null, sz));
};

let serializeDotField = function(staticCtx, df) {
	return join2(serializeExpression(staticCtx, df[2]), create0(15, df[5], df[7]));
};

let serializeDoWhileLoop = function(staticCtx, doWhileLoop) {
	let body = serializeCodeBlock(staticCtx, doWhileLoop[11]);
	let condition = ByteCodeUtil_ensureBooleanExpression(doWhileLoop[3][0], serializeExpression(staticCtx, doWhileLoop[3]));
	return join4(finalizeBreakContinue(body, condition[0] + 2, true, 0), condition, create1(28, null, null, 1), create1(24, null, null, -(2 + condition[0] + body[0])));
};

let serializeExpression = function(staticCtx, expr) {
	switch (expr[1]) {
		case 15:
			fail("");
			return null;
		case 2:
			return serializeBaseCtorReference(expr);
		case 3:
			return serializeBinaryOp(staticCtx, expr);
		case 4:
			return serializeBitwiseNot(staticCtx, expr);
		case 5:
			return serializeBoolConst(expr);
		case 6:
			return serializeBooleanNot(staticCtx, expr);
		case 7:
			return serializeClassReference(expr);
		case 8:
			return serializeConstructorInvocation(staticCtx, expr);
		case 10:
			return serializeDictionaryDefinition(staticCtx, expr);
		case 11:
			return serializeDotField(staticCtx, expr);
		case 14:
			return serializeExtensionInvocation(staticCtx, expr);
		case 16:
			return serializeFloatConstant(expr);
		case 17:
			return serializeFunctionInvocation(staticCtx, expr);
		case 18:
			return serializeFunctionReference(expr);
		case 20:
			return serializeIndex(staticCtx, expr);
		case 21:
			return serializeInlineIncrement(staticCtx, expr);
		case 22:
			return serializeIntegerConstant(expr);
		case 23:
			return serializeLambda(expr);
		case 24:
			return serializeListDefinition(staticCtx, expr);
		case 25:
			return serializeNegativeSign(staticCtx, expr);
		case 26:
			return serializeNullConstant(expr);
		case 27:
			return serializeSlice(staticCtx, expr);
		case 28:
			return serializeStringConstant(expr);
		case 29:
			return serializeTernary(staticCtx, expr);
		case 30:
			return serializeThis(expr);
		case 33:
			return serializeTypeOf(staticCtx, expr);
		case 31:
			return serializeVariable(expr);
		default:
			fail("Not implemented");
			return null;
	}
};

let serializeExpressionStatement = function(staticCtx, exprStmnt) {
	return join2(serializeExpression(staticCtx, exprStmnt[2]), create0(27, null, null));
};

let serializeExtensionInvocation = function(staticCtx, extInvoke) {
	if (SpecialActionUtil_IsSpecialActionAndNotExtension(staticCtx[2], extInvoke[7])) {
		return serializeSpecialAction(staticCtx, extInvoke);
	}
	let buf = null;
	let argc = extInvoke[12].length;
	let i = 0;
	while (i < argc) {
		buf = join2(buf, serializeExpression(staticCtx, extInvoke[12][i]));
		i++;
	}
	return join2(buf, create1(20, extInvoke[0], extInvoke[7], argc));
};

let serializeFloatConstant = function(floatConst) {
	let val = floatConst[9];
	if (val * 4 % 1 == 0) {
		return create1(38, null, null, val * 4);
	}
	return create0(38, null, FloatToStringWorkaround(val));
};

let serializeForEachLoop = function(staticCtx, forEachLoop) {
	let loopExpr = "@fe" + (forEachLoop[16] + '');
	let iteratorVar = "@fi" + (forEachLoop[16] + '');
	let setup = join5(serializeExpression(staticCtx, forEachLoop[2]), create0(66, forEachLoop[2][0], null), create0(3, null, loopExpr), create1(40, null, null, 0), create0(3, null, iteratorVar));
	let bufBody = serializeCodeBlock(staticCtx, forEachLoop[11]);
	let increment = join4(create0(45, null, iteratorVar), create1(40, null, null, 1), create0(4, null, "+"), create0(3, null, iteratorVar));
	let doAssign = join4(create0(45, null, loopExpr), create0(45, null, iteratorVar), create0(22, null, null), create0(3, forEachLoop[7], forEachLoop[7][0]));
	let lengthCheck = join5(create0(45, null, iteratorVar), create0(45, null, loopExpr), create0(15, null, "length"), create0(4, null, ">="), create1(29, null, null, doAssign[0] + bufBody[0] + increment[0] + 1));
	bufBody = finalizeBreakContinue(bufBody, 5, true, 0);
	let reverseJumpDistance = -1 - increment[0] - bufBody[0] - doAssign[0] - lengthCheck[0];
	let fullCode = join6(setup, lengthCheck, doAssign, bufBody, increment, create1(24, null, null, reverseJumpDistance));
	return fullCode;
};

let serializeForLoop = function(staticCtx, forLoop) {
	let condition = forLoop[3];
	let code = forLoop[11];
	let init = forLoop[9];
	let step = forLoop[10];
	let bufInit = serializeCodeBlock(staticCtx, init);
	let bufStep = serializeCodeBlock(staticCtx, step);
	let bufBody = serializeCodeBlock(staticCtx, code);
	let bufCondition = serializeExpression(staticCtx, condition);
	bufCondition = ByteCodeUtil_ensureBooleanExpression(condition[0], bufCondition);
	let stepSize = 0;
	let bodySize = 0;
	let conditionSize = bufCondition[0];
	if (bufStep != null) {
		stepSize = bufStep[0];
	}
	if (bufBody != null) {
		bodySize = bufBody[0];
	}
	return join6(bufInit, bufCondition, create1(28, null, null, bodySize + stepSize + 1), finalizeBreakContinue(bufBody, bufStep[0] + 1, true, 0), bufStep, create1(24, null, null, -(1 + bodySize + stepSize + 1 + conditionSize)));
};

let serializeFunctionInvocation = function(staticCtx, funcInvoke) {
	let buf = serializeExpression(staticCtx, funcInvoke[2]);
	let argc = funcInvoke[12].length;
	let i = 0;
	while (i < argc) {
		buf = join2(buf, serializeExpression(staticCtx, funcInvoke[12][i]));
		i += 1;
	}
	return join2(buf, create1(21, funcInvoke[5], null, argc));
};

let serializeFunctionReference = function(funcRef) {
	let funcDef = funcRef[10];
	let index = funcDef[10];
	if (index == -1) {
		fail("");
	}
	return create1(39, funcRef[0], null, index);
};

let serializeIfStatement = function(staticCtx, ifStmnt) {
	let condition = ifStmnt[3];
	let ifCode = ifStmnt[11];
	let elseCode = ifStmnt[12];
	let buf = serializeExpression(staticCtx, condition);
	let bufTrue = serializeCodeBlock(staticCtx, ifCode);
	let bufFalse = serializeCodeBlock(staticCtx, elseCode);
	buf = ByteCodeUtil_ensureBooleanExpression(condition[0], buf);
	let trueSize = 0;
	let falseSize = 0;
	if (bufTrue != null) {
		trueSize = bufTrue[0];
	}
	if (bufFalse != null) {
		falseSize = bufFalse[0];
	}
	if (trueSize + falseSize == 0) {
		return join2(buf, create0(27, null, null));
	}
	if (falseSize == 0) {
		return join3(buf, create1(28, null, null, trueSize), bufTrue);
	}
	if (trueSize == 0) {
		return join3(buf, create1(29, null, null, falseSize), bufFalse);
	}
	return join5(buf, create1(28, null, null, trueSize + 1), bufTrue, create1(24, null, null, falseSize), bufFalse);
};

let serializeIndex = function(staticCtx, index) {
	return join3(serializeExpression(staticCtx, index[2]), serializeExpression(staticCtx, index[4]), create0(22, index[5], null));
};

let serializeInlineIncrement = function(staticCtx, ii) {
	switch (ii[2][1]) {
		case 31:
			return serializeInlineIncrementVar(staticCtx, ii);
		case 20:
			return serializeInlineIncrementIndex(staticCtx, ii);
		case 11:
			return serializeInlineIncrementDotField(staticCtx, ii);
	}
	fail("");
	return null;
};

let serializeInlineIncrementDotField = function(staticCtx, ii) {
	let isPrefix = ii[6];
	let root = serializeExpression(staticCtx, ii[2][2]);
	let incrAmt = 1;
	if (ii[5][0] == "--") {
		incrAmt = -1;
	}
	let buf = root;
	buf = join2(buf, create0(52, null, null));
	buf = join2(buf, create0(15, ii[2][5], ii[2][7]));
	if (isPrefix) {
		buf = join2(buf, create1(23, ii[5], null, incrAmt));
		buf = join2(buf, create0(50, null, null));
	} else {
		buf = join2(buf, create0(52, null, null));
		buf = join2(buf, create1(23, ii[5], null, incrAmt));
		buf = join2(buf, create0(51, null, null));
	}
	buf = join2(buf, create0(1, ii[5], ii[2][7]));
	return buf;
};

let serializeInlineIncrementIndex = function(staticCtx, ii) {
	let isPrefix = ii[6];
	let root = serializeExpression(staticCtx, ii[2][2]);
	let index = serializeExpression(staticCtx, ii[2][4]);
	let incrAmt = 1;
	if (ii[5][0] == "--") {
		incrAmt = -1;
	}
	let buf = join2(root, index);
	buf = join2(buf, create0(53, null, null));
	buf = join2(buf, create0(22, ii[2][5], null));
	if (isPrefix) {
		buf = join2(buf, create1(23, ii[5], null, incrAmt));
		buf = join2(buf, create0(52, null, null));
	} else {
		buf = join2(buf, create0(52, null, null));
		buf = join2(buf, create1(23, ii[5], null, incrAmt));
	}
	buf = join2(buf, create0(49, null, null));
	buf = join2(buf, create0(2, ii[2][5], null));
	return buf;
};

let serializeInlineIncrementVar = function(staticCtx, ii) {
	let isPrefix = ii[6];
	let incrAmt = 1;
	if (ii[5][0] == "--") {
		incrAmt = -1;
	}
	if (isPrefix) {
		return join4(serializeExpression(staticCtx, ii[2]), create1(23, ii[5], null, incrAmt), create0(52, null, null), create0(3, null, ii[2][7]));
	} else {
		return join4(serializeExpression(staticCtx, ii[2]), create0(52, null, null), create1(23, ii[5], null, incrAmt), create0(3, null, ii[2][7]));
	}
};

let serializeIntegerConstant = function(intConst) {
	return create1(40, intConst[0], null, intConst[8]);
};

let serializeLambda = function(lambda) {
	let lambdaEntity = lambda[10][2];
	return create1(63, lambda[0], null, lambdaEntity[3][10]);
};

let serializeListDefinition = function(staticCtx, listDef) {
	let buf = null;
	let sz = listDef[14].length;
	let i = 0;
	while (i < sz) {
		buf = join2(buf, serializeExpression(staticCtx, listDef[14][i]));
		i += 1;
	}
	return join2(buf, create1(12, listDef[0], null, sz));
};

let serializeNegativeSign = function(staticCtx, negSign) {
	return join2(serializeExpression(staticCtx, negSign[2]), create0(26, negSign[5], null));
};

let serializeNullConstant = function(nullConst) {
	return create0(41, nullConst[0], null);
};

let serializeReturn = function(staticCtx, ret) {
	let buf = null;
	if (ret[2] == null) {
		buf = create0(41, null, null);
	} else {
		buf = serializeExpression(staticCtx, ret[2]);
	}
	return join2(buf, create0(46, ret[0], null));
};

let serializeSlice = function(staticCtx, slice) {
	let start = slice[12][0];
	let end = slice[12][1];
	let step = slice[12][2];
	let sliceMask = 0;
	let rootBuf = serializeExpression(staticCtx, slice[2]);
	let startBuf = null;
	let endBuf = null;
	let stepBuf = null;
	if (start != null) {
		sliceMask += 1;
		startBuf = ByteCodeUtil_ensureIntegerExpression(start[0], serializeExpression(staticCtx, start));
	}
	if (end != null) {
		sliceMask += 2;
		endBuf = ByteCodeUtil_ensureIntegerExpression(end[0], serializeExpression(staticCtx, end));
	}
	if (step != null) {
		sliceMask += 4;
		stepBuf = ByteCodeUtil_ensureIntegerExpression(step[0], serializeExpression(staticCtx, step));
	}
	return join5(rootBuf, startBuf, endBuf, stepBuf, create1(47, slice[5], null, sliceMask));
};

let serializeSpecialAction = function(staticCtx, action) {
	let argBuffer = null;
	let argc = action[12].length;
	let i = 0;
	while (i < argc) {
		argBuffer = join2(argBuffer, serializeExpression(staticCtx, action[12][i]));
		i++;
	}
	if (action[7] == "math_floor") {
		return join2(argBuffer, create0(25, action[0], null));
	}
	if (action[7] == "unix_time") {
		return create2(48, null, null, 1, action[12][0][8]);
	}
	let specialActionOpCode = SpecialActionUtil_GetSpecialActionOpCode(staticCtx[2], action[7]);
	let actionBuf = create1(48, null, null, specialActionOpCode);
	return join2(argBuffer, actionBuf);
};

let serializeStatement = function(staticCtx, stmnt) {
	switch (stmnt[1]) {
		case 1:
			let op = stmnt[6][0];
			if (op != "=") {
				op = op.substring(0, 0 + (op.length - 1));
			}
			switch (stmnt[4][1]) {
				case 31:
					return serializeAssignVariable(staticCtx, stmnt, op);
				case 20:
					return serializeAssignIndex(staticCtx, stmnt, op);
				case 11:
					return serializeAssignField(staticCtx, stmnt, op);
			}
			fail("");
			return null;
		case 2:
			return serializeBreak(stmnt);
		case 3:
			return serializeContinue(stmnt);
		case 4:
			return serializeDoWhileLoop(staticCtx, stmnt);
		case 5:
			return serializeExpressionStatement(staticCtx, stmnt);
		case 6:
			return serializeForLoop(staticCtx, stmnt);
		case 7:
			return serializeForEachLoop(staticCtx, stmnt);
		case 8:
			return serializeIfStatement(staticCtx, stmnt);
		case 9:
			return serializeReturn(staticCtx, stmnt);
		case 10:
			return serializeSwitchStatement(staticCtx, stmnt);
		case 11:
			return serializeThrowStatement(staticCtx, stmnt);
		case 12:
			return serializeTryStatement(staticCtx, stmnt);
		case 13:
			return serializeWhileLoop(staticCtx, stmnt);
		default:
			fail("");
			return null;
	}
};

let serializeStringConstant = function(strConst) {
	return create0(42, strConst[0], strConst[7]);
};

let serializeSwitchStatement = function(staticCtx, switchStmnt) {
	let i = 0;
	let j = 0;
	if (switchStmnt[14].length == 0) {
		return join3(serializeExpression(staticCtx, switchStmnt[3]), create0(17, switchStmnt[3][0], null), create0(27, null, null));
	}
	let conditionTypeEnsuranceOpCode = 17;
	let firstCaseExpr = switchStmnt[14][0][1][0];
	if (firstCaseExpr != null) {
		if (firstCaseExpr[1] == 22) {
			conditionTypeEnsuranceOpCode = 18;
		} else if (firstCaseExpr[1] == 28) {
			conditionTypeEnsuranceOpCode = 19;
		} else {
			fail("");
		}
	}
	let condBuf = join2(serializeExpression(staticCtx, switchStmnt[3]), create0(conditionTypeEnsuranceOpCode, switchStmnt[3][0], null));
	let caseBuf = null;
	let currentJumpOffset = 0;
	let hasDefault = false;
	let stringJumpOffset = {};
	let intJumpOffset = {};
	let defaultJumpOffset = -1;
	i = 0;
	while (i < switchStmnt[14].length) {
		let chunk = switchStmnt[14][i];
		j = 0;
		while (j < chunk[1].length) {
			let expr = chunk[1][j];
			if (expr == null) {
				defaultJumpOffset = currentJumpOffset;
				hasDefault = true;
			} else if (conditionTypeEnsuranceOpCode == 18) {
				intJumpOffset[expr[8]] = currentJumpOffset;
			} else if (conditionTypeEnsuranceOpCode == 19) {
				stringJumpOffset[expr[7]] = currentJumpOffset;
			} else {
				fail("");
			}
			j += 1;
		}
		let chunkBuf = serializeCodeBlock(staticCtx, [...(chunk[2])]);
		currentJumpOffset += chunkBuf[0];
		caseBuf = join2(caseBuf, chunkBuf);
		i += 1;
	}
	if (!hasDefault) {
		defaultJumpOffset = currentJumpOffset;
	}
	caseBuf = finalizeBreakContinue(caseBuf, 0, false, 0);
	let jumpBuf = null;
	if (conditionTypeEnsuranceOpCode == 18) {
		let jumpArgs = [];
		jumpArgs.push(-1);
		let intKeys = PST$sortedCopyOfArray(Object.keys(intJumpOffset));
		i = 0;
		while (i < intKeys.length) {
			let k = intKeys[i];
			jumpArgs.push(intJumpOffset[k] + 2);
			jumpArgs.push(k);
			i += 1;
		}
		let lookup = create0(54, null, null);
		lookup[4][3] = [...(jumpArgs)];
		jumpBuf = join3(create2(55, null, null, defaultJumpOffset + 2, 1), lookup, create1(56, null, null, 2));
	} else if (conditionTypeEnsuranceOpCode == 19) {
		let keys = PST$sortedCopyOfArray(Object.keys(stringJumpOffset));
		jumpBuf = create2(55, null, null, defaultJumpOffset + keys.length + 1, 2);
		i = 0;
		while (i < keys.length) {
			let key = keys[i];
			jumpBuf = join2(jumpBuf, create1(54, null, key, stringJumpOffset[key] + keys.length + 1));
			i += 1;
		}
		jumpBuf = join2(jumpBuf, create1(56, null, null, jumpBuf[0]));
	} else {
		jumpBuf = create0(27, null, null);
	}
	return join3(condBuf, jumpBuf, caseBuf);
};

let serializeTernary = function(staticCtx, ternaryExpression) {
	let condBuf = serializeExpression(staticCtx, ternaryExpression[2]);
	let leftBuf = serializeExpression(staticCtx, ternaryExpression[3]);
	let rightBuf = serializeExpression(staticCtx, ternaryExpression[4]);
	condBuf = ByteCodeUtil_ensureBooleanExpression(ternaryExpression[5], condBuf);
	return join5(condBuf, create1(28, null, null, leftBuf[0] + 1), leftBuf, create1(24, null, null, rightBuf[0]), rightBuf);
};

let serializeThis = function(thisKeyword) {
	return create0(43, thisKeyword[0], null);
};

let serializeThrowStatement = function(staticCtx, thrw) {
	return join2(serializeExpression(staticCtx, thrw[2]), create0(59, thrw[0], null));
};

let serializeTryStatement = function(staticCtx, tryStmnt) {
	let i = 0;
	let tryBuf = serializeCodeBlock(staticCtx, tryStmnt[11]);
	let catchBufs = [];
	i = 0;
	while (i < tryStmnt[15].length) {
		let cc = tryStmnt[15][i];
		let catchBuf = serializeCodeBlock(staticCtx, cc[3]);
		if (cc[2] == null) {
			catchBuf = join2(create0(27, null, null), catchBuf);
		} else {
			catchBuf = join2(create0(3, cc[2], cc[2][0]), catchBuf);
		}
		catchBufs.push(catchBuf);
		i += 1;
	}
	let finallyBuf = join2(serializeCodeBlock(staticCtx, tryStmnt[13]), create0(60, null, null));
	let jumpOffset = 0;
	i = catchBufs.length - 2;
	while (i >= 0) {
		jumpOffset += catchBufs[i + 1][0];
		catchBufs[i] = join2(catchBufs[i], create1(24, null, null, jumpOffset));
		i -= 1;
	}
	jumpOffset = 0;
	let catchRouterArgs = [];
	i = 0;
	while (i < catchBufs.length) {
		if (i > 0) {
			catchRouterArgs.push(0);
		}
		catchRouterArgs.push(jumpOffset);
		let cc = tryStmnt[15][i];
		let j = 0;
		while (j < cc[1].length) {
			let cdef = cc[1][j];
			catchRouterArgs.push(cdef[6][10]);
			j += 1;
		}
		jumpOffset += catchBufs[i][0];
		i += 1;
	}
	catchRouterArgs.splice(0, 0, jumpOffset);
	let catchRouterBuf = ByteCodeBuffer_fromRow(ByteCodeRow_new(61, null, null, [...(catchRouterArgs)]));
	let routeAndCatches = catchRouterBuf;
	i = 0;
	while (i < catchBufs.length) {
		let catchBuf = catchBufs[i];
		routeAndCatches = join2(routeAndCatches, catchBuf);
		i += 1;
	}
	tryBuf = join2(tryBuf, create1(24, null, null, routeAndCatches[0]));
	let tryCatchInfo = PST$createNewArray(4);
	tryCatchInfo[0] = 0;
	tryCatchInfo[1] = tryBuf[0];
	tryCatchInfo[2] = routeAndCatches[0];
	tryCatchInfo[3] = finallyBuf[0];
	tryBuf[5][6] = tryCatchInfo;
	return join3(tryBuf, routeAndCatches, finallyBuf);
};

let serializeTypeOf = function(staticCtx, typeOfExpr) {
	let root = serializeExpression(staticCtx, typeOfExpr[2]);
	return join2(root, create0(65, typeOfExpr[0], null));
};

let serializeVariable = function(v) {
	if (v[7] == "print") {
		fail("");
	}
	return create0(45, v[0], v[7]);
};

let serializeWhileLoop = function(staticCtx, whileLoop) {
	let condBuf = serializeExpression(staticCtx, whileLoop[3]);
	condBuf = ByteCodeUtil_ensureBooleanExpression(whileLoop[3][0], condBuf);
	let loopBody = serializeCodeBlock(staticCtx, whileLoop[11]);
	return join4(condBuf, create1(28, null, null, loopBody[0] + 1), finalizeBreakContinue(loopBody, 1, true, -loopBody[0] - 1 - condBuf[0]), create1(24, null, null, -(loopBody[0] + condBuf[0] + 1 + 1)));
};

let SimpleExprToTypeName = function(t) {
	switch (t) {
		case 5:
			return "boolean";
		case 22:
			return "integer";
		case 16:
			return "float";
		case 26:
			return "null";
		case 28:
			return "string";
		case 12:
			return "enum";
	}
	fail("not implemented");
	return null;
};

let SpecialActionUtil_GetSpecialActionArgc = function(sau, name) {
	return sau[1][name];
};

let SpecialActionUtil_GetSpecialActionOpCode = function(sau, name) {
	return sau[0][name];
};

let SpecialActionUtil_IsSpecialActionAndNotExtension = function(sau, name) {
	return sau[0][name] !== undefined;
};

let SpecialActionUtil_new = function() {
	let idByName = {};
	idByName["b64_from_bytes"] = 16;
	idByName["b64_to_bytes"] = 17;
	idByName["cmp"] = 6;
	idByName["json_parse"] = 20;
	idByName["json_serialize"] = 21;
	idByName["math_arccos"] = 8;
	idByName["math_arcsin"] = 9;
	idByName["math_arctan"] = 10;
	idByName["math_cos"] = 11;
	idByName["math_floor"] = -1;
	idByName["math_log"] = 12;
	idByName["math_sin"] = 13;
	idByName["math_tan"] = 14;
	idByName["parse_float"] = 25;
	idByName["parse_int"] = 15;
	idByName["random_float"] = 7;
	idByName["sort_end"] = 3;
	idByName["sort_get_next_cmp"] = 4;
	idByName["sort_proceed"] = 5;
	idByName["sort_start"] = 2;
	idByName["txt_bytes_to_string"] = 23;
	idByName["txt_is_valid_enc"] = 22;
	idByName["txt_string_to_bytes"] = 24;
	idByName["unix_time"] = 1;
	idByName["xml_parse"] = 26;
	let argcByName = {};
	argcByName["b64_from_bytes"] = 2;
	argcByName["b64_to_bytes"] = 1;
	argcByName["cmp"] = 2;
	argcByName["json_parse"] = 2;
	argcByName["json_serialize"] = 2;
	argcByName["math_arccos"] = 1;
	argcByName["math_arcsin"] = 1;
	argcByName["math_arctan"] = 2;
	argcByName["math_cos"] = 1;
	argcByName["math_floor"] = 1;
	argcByName["math_log"] = 2;
	argcByName["math_sin"] = 1;
	argcByName["math_tan"] = 1;
	argcByName["parse_float"] = 1;
	argcByName["parse_int"] = 1;
	argcByName["random_float"] = 0;
	argcByName["sort_end"] = 1;
	argcByName["sort_get_next_cmp"] = 2;
	argcByName["sort_proceed"] = 2;
	argcByName["sort_start"] = 2;
	argcByName["txt_bytes_to_string"] = 2;
	argcByName["txt_is_valid_enc"] = 1;
	argcByName["txt_string_to_bytes"] = 2;
	argcByName["unix_time"] = 1;
	argcByName["xml_parse"] = 2;
	return [idByName, argcByName];
};

let Statement_createAssignment = function(targetExpr, assignOp, valueExpr) {
	let assign = Statement_new(targetExpr[0], 1);
	assign[4] = targetExpr;
	assign[5] = valueExpr;
	assign[6] = assignOp;
	return assign;
};

let Statement_createBreakContinue = function(breakContinueToken) {
	let type = 3;
	if (breakContinueToken[0] == "break") {
		type = 2;
	}
	return Statement_new(breakContinueToken, type);
};

let Statement_createDoWhile = function(doToken, code, whileToken, condition) {
	let doWhile = Statement_new(doToken, 4);
	doWhile[3] = condition;
	doWhile[11] = code;
	doWhile[6] = whileToken;
	return doWhile;
};

let Statement_createExpressionAsStatement = function(expr) {
	let wrapper = Statement_new(expr[0], 5);
	wrapper[2] = expr;
	return wrapper;
};

let Statement_createForEachLoop = function(forToken, varName, listExpr, code) {
	let forEachLoop = Statement_new(forToken, 7);
	forEachLoop[7] = varName;
	forEachLoop[2] = listExpr;
	forEachLoop[11] = code;
	return forEachLoop;
};

let Statement_createForLoop = function(forToken, init, condition, step, code) {
	let forLoop = Statement_new(forToken, 6);
	forLoop[3] = condition;
	forLoop[9] = init;
	forLoop[10] = step;
	forLoop[11] = code;
	return forLoop;
};

let Statement_createIfStatement = function(ifToken, condition, ifCode, elseCode) {
	let ifStatement = Statement_new(ifToken, 8);
	ifStatement[3] = condition;
	ifStatement[11] = ifCode;
	ifStatement[12] = elseCode;
	return ifStatement;
};

let Statement_createReturn = function(returnToken, expr) {
	let ret = Statement_new(returnToken, 9);
	ret[2] = expr;
	return ret;
};

let Statement_createSwitchStatement = function(switchToken, condition, chunks) {
	let swtStmnt = Statement_new(switchToken, 10);
	swtStmnt[3] = condition;
	swtStmnt[14] = chunks;
	return swtStmnt;
};

let Statement_createThrow = function(throwToken, value) {
	let throwStmnt = Statement_new(throwToken, 11);
	throwStmnt[2] = value;
	return throwStmnt;
};

let Statement_createTry = function(tryToken, tryCode, catches, finallyToken, finallyCode) {
	let tryStmnt = Statement_new(tryToken, 12);
	tryStmnt[11] = tryCode;
	tryStmnt[15] = catches;
	tryStmnt[13] = finallyCode;
	tryStmnt[8] = finallyToken;
	return tryStmnt;
};

let Statement_createWhileLoop = function(whileToken, condition, code) {
	let whileLoop = Statement_new(whileToken, 13);
	whileLoop[3] = condition;
	whileLoop[11] = code;
	return whileLoop;
};

let Statement_new = function(firstToken, type) {
	return [firstToken, type, null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0];
};

let StatementParser_IdentifyKeywordType = function(kw) {
	switch (kw.charAt(0)) {
		case "b":
			if (kw == "break") {
				return 1;
			}
			break;
		case "c":
			if (kw == "continue") {
				return 2;
			}
			break;
		case "d":
			if (kw == "do") {
				return 3;
			}
			break;
		case "f":
			if (kw == "for") {
				return 4;
			}
			break;
		case "i":
			if (kw == "if") {
				return 5;
			}
			break;
		case "r":
			if (kw == "return") {
				return 6;
			}
			break;
		case "s":
			if (kw == "switch") {
				return 7;
			}
			break;
		case "t":
			if (kw == "throw") {
				return 8;
			}
			if (kw == "try") {
				return 9;
			}
			break;
		case "w":
			if (kw == "while") {
				return 10;
			}
			break;
		case "y":
			if (kw == "yield") {
				return 11;
			}
			break;
	}
	return 0;
};

let StatementResolver_FirstPass_Assignment = function(resolver, assign) {
	assign[4] = ExpressionResolver_ResolveExpressionFirstPass(resolver, assign[4]);
	assign[5] = ExpressionResolver_ResolveExpressionFirstPass(resolver, assign[5]);
	if (assign[4][1] == 31) {
		resolver[7][2][5][assign[4][7]] = true;
	}
	return assign;
};

let StatementResolver_FirstPass_Break = function(resolver, br) {
	if (resolver[9] == null) {
		Errors_Throw(br[0], "The 'break' keyword can only be used inside loops and switch statements.");
	} else if (resolver[9][1] == 12) {
		Errors_Throw(br[0], "The 'break' keyword cannot be used inside a try/catch/finally block");
	}
	return br;
};

let StatementResolver_FirstPass_Continue = function(resolver, cont) {
	if (resolver[9] == null) {
		Errors_Throw(cont[0], "The 'continue' keyword can only be used inside loops.");
	} else if (resolver[9][1] == 10) {
		Errors_Throw(cont[0], "The 'continue' keyword cannot be used in switch statements, even if nested in a loop.");
	} else if (resolver[9][1] == 12) {
		Errors_Throw(cont[0], "The 'continue' keyword cannot be used inside a try/catch/finally block");
	}
	return cont;
};

let StatementResolver_FirstPass_DoWhileLoop = function(resolver, doWhileLoop) {
	let oldBreakContext = resolver[9];
	resolver[9] = doWhileLoop;
	StatementResolver_ResolveStatementArrayFirstPass(resolver, doWhileLoop[11]);
	resolver[9] = oldBreakContext;
	doWhileLoop[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, doWhileLoop[3]);
	return doWhileLoop;
};

let StatementResolver_FirstPass_ForEachLoop = function(resolver, forEachLoop) {
	forEachLoop[16] = EntityResolver_GetNextAutoVarId(resolver);
	forEachLoop[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, forEachLoop[2]);
	resolver[7][2][5][forEachLoop[7][0]] = true;
	StatementResolver_ResolveStatementArrayFirstPass(resolver, forEachLoop[11]);
	return forEachLoop;
};

let StatementResolver_FirstPass_ForLoop = function(resolver, forLoop) {
	StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop[9]);
	forLoop[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, forLoop[3]);
	StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop[10]);
	let oldBreakContext = resolver[9];
	resolver[9] = forLoop;
	StatementResolver_ResolveStatementArrayFirstPass(resolver, forLoop[11]);
	resolver[9] = oldBreakContext;
	return forLoop;
};

let StatementResolver_FirstPass_IfStatement = function(resolver, ifStatement) {
	ifStatement[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, ifStatement[3]);
	StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement[11]);
	StatementResolver_ResolveStatementArrayFirstPass(resolver, ifStatement[12]);
	return ifStatement;
};

let StatementResolver_FirstPass_SwitchStatement = function(resolver, switchStmnt) {
	let j = 0;
	let oldBreakContext = resolver[9];
	resolver[9] = switchStmnt;
	switchStmnt[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, switchStmnt[3]);
	let i = 0;
	while (i < switchStmnt[14].length) {
		let chunk = switchStmnt[14][i];
		j = 0;
		while (j < chunk[1].length) {
			let expr = chunk[1][j];
			if (expr != null) {
				chunk[1][j] = ExpressionResolver_ResolveExpressionFirstPass(resolver, expr);
			}
			j += 1;
		}
		j = 0;
		while (j < chunk[2].length) {
			chunk[2][j] = StatementResolver_ResolveStatementFirstPass(resolver, chunk[2][j]);
			j += 1;
		}
		i += 1;
	}
	resolver[9] = oldBreakContext;
	return switchStmnt;
};

let StatementResolver_FirstPass_Try = function(resolver, tryStatement) {
	let oldBreakContext = resolver[9];
	resolver[9] = tryStatement;
	StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement[11]);
	let i = 0;
	while (i < tryStatement[15].length) {
		let cc = tryStatement[15][i];
		if (cc[2] != null) {
			resolver[7][2][5][cc[2][0]] = true;
		}
		if (cc[0].length > 0) {
			fail("Not implemented");
		} else {
			cc[4] = true;
			cc[1] = PST$createNewArray(0);
		}
		StatementResolver_ResolveStatementArrayFirstPass(resolver, cc[3]);
		i += 1;
	}
	StatementResolver_ResolveStatementArrayFirstPass(resolver, tryStatement[13]);
	resolver[9] = oldBreakContext;
	return tryStatement;
};

let StatementResolver_FirstPass_WhileLoop = function(resolver, whileLoop) {
	whileLoop[3] = ExpressionResolver_ResolveExpressionFirstPass(resolver, whileLoop[3]);
	let oldBreakContext = resolver[9];
	resolver[9] = whileLoop;
	StatementResolver_ResolveStatementArrayFirstPass(resolver, whileLoop[11]);
	resolver[9] = oldBreakContext;
	return whileLoop;
};

let StatementResolver_ResolveStatementArrayFirstPass = function(resolver, arr) {
	let i = 0;
	while (i < arr.length) {
		arr[i] = StatementResolver_ResolveStatementFirstPass(resolver, arr[i]);
		i += 1;
	}
};

let StatementResolver_ResolveStatementArraySecondPass = function(resolver, arr) {
	let i = 0;
	while (i < arr.length) {
		arr[i] = StatementResolver_ResolveStatementSecondPass(resolver, arr[i]);
		i += 1;
	}
};

let StatementResolver_ResolveStatementFirstPass = function(resolver, s) {
	switch (s[1]) {
		case 1:
			return StatementResolver_FirstPass_Assignment(resolver, s);
		case 2:
			return StatementResolver_FirstPass_Break(resolver, s);
		case 3:
			return StatementResolver_FirstPass_Continue(resolver, s);
		case 4:
			return StatementResolver_FirstPass_DoWhileLoop(resolver, s);
		case 7:
			return StatementResolver_FirstPass_ForEachLoop(resolver, s);
		case 6:
			return StatementResolver_FirstPass_ForLoop(resolver, s);
		case 8:
			return StatementResolver_FirstPass_IfStatement(resolver, s);
		case 10:
			return StatementResolver_FirstPass_SwitchStatement(resolver, s);
		case 12:
			return StatementResolver_FirstPass_Try(resolver, s);
		case 13:
			return StatementResolver_FirstPass_WhileLoop(resolver, s);
		case 9:
			s[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, s[2]);
			break;
		case 11:
			s[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, s[2]);
			break;
		case 5:
			s[2] = ExpressionResolver_ResolveExpressionFirstPass(resolver, s[2]);
			break;
		default:
			fail("Not implemented");
			break;
	}
	return s;
};

let StatementResolver_ResolveStatementSecondPass = function(resolver, s) {
	switch (s[1]) {
		case 1:
			return StatementResolver_SecondPass_Assignment(resolver, s);
		case 2:
			return StatementResolver_SecondPass_Break(resolver, s);
		case 3:
			return StatementResolver_SecondPass_Continue(resolver, s);
		case 4:
			return StatementResolver_SecondPass_DoWhileLoop(resolver, s);
		case 5:
			return StatementResolver_SecondPass_ExpressionAsStatement(resolver, s);
		case 6:
			return StatementResolver_SecondPass_ForLoop(resolver, s);
		case 7:
			return StatementResolver_SecondPass_ForEachLoop(resolver, s);
		case 8:
			return StatementResolver_SecondPass_IfStatement(resolver, s);
		case 9:
			return StatementResolver_SecondPass_Return(resolver, s);
		case 10:
			return StatementResolver_SecondPass_SwitchStatement(resolver, s);
		case 11:
			return StatementResolver_SecondPass_ThrowStatement(resolver, s);
		case 12:
			return StatementResolver_SecondPass_TryStatement(resolver, s);
		case 13:
			return StatementResolver_SecondPass_WhileLoop(resolver, s);
	}
	fail("Not implemented");
	return null;
};

let StatementResolver_SecondPass_Assignment = function(resolver, assignment) {
	assignment[4] = ExpressionResolver_ResolveExpressionSecondPass(resolver, assignment[4]);
	assignment[5] = ExpressionResolver_ResolveExpressionSecondPass(resolver, assignment[5]);
	let target = assignment[4];
	switch (target[1]) {
		case 20:
			break;
		case 11:
			break;
		case 31:
			break;
		default:
			Errors_Throw(assignment[6], "Invalid assignment. Cannot assign to this type of expression.");
			break;
	}
	return assignment;
};

let StatementResolver_SecondPass_Break = function(resolver, br) {
	return br;
};

let StatementResolver_SecondPass_Continue = function(resolver, cont) {
	return cont;
};

let StatementResolver_SecondPass_DoWhileLoop = function(resolver, doWhileLoop) {
	let oldBreakContext = resolver[9];
	resolver[9] = doWhileLoop;
	StatementResolver_ResolveStatementArraySecondPass(resolver, doWhileLoop[11]);
	doWhileLoop[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, doWhileLoop[3]);
	resolver[9] = oldBreakContext;
	return doWhileLoop;
};

let StatementResolver_SecondPass_ExpressionAsStatement = function(resolver, exprAsStmnt) {
	exprAsStmnt[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, exprAsStmnt[2]);
	switch (exprAsStmnt[2][1]) {
		case 17:
			break;
		case 14:
			break;
		case 21:
			break;
		default:
			Errors_Throw(exprAsStmnt[0], "This type of expression cannot exist by itself. Did you mean to assign it to a variable?");
			break;
	}
	return exprAsStmnt;
};

let StatementResolver_SecondPass_ForEachLoop = function(resolver, forEachLoop) {
	forEachLoop[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, forEachLoop[2]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, forEachLoop[11]);
	return forEachLoop;
};

let StatementResolver_SecondPass_ForLoop = function(resolver, forLoop) {
	let oldBreakContext = resolver[9];
	resolver[9] = forLoop;
	StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop[9]);
	forLoop[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, forLoop[3]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop[10]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, forLoop[11]);
	resolver[9] = oldBreakContext;
	return forLoop;
};

let StatementResolver_SecondPass_IfStatement = function(resolver, ifStatement) {
	ifStatement[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ifStatement[3]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, ifStatement[11]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, ifStatement[12]);
	return ifStatement;
};

let StatementResolver_SecondPass_Return = function(resolver, ret) {
	ret[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, ret[2]);
	return ret;
};

let StatementResolver_SecondPass_SwitchStatement = function(resolver, switchStmnt) {
	switchStmnt[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, switchStmnt[3]);
	let exprType = -1;
	let strCollisions = {};
	let intCollisions = {};
	let j = 0;
	let i = 0;
	while (i < switchStmnt[14].length) {
		let chunk = switchStmnt[14][i];
		j = 0;
		while (j < chunk[1].length) {
			let expr = chunk[1][j];
			if (expr != null) {
				expr = ExpressionResolver_ResolveExpressionSecondPass(resolver, expr);
				chunk[1][j] = expr;
				if (!IsExpressionConstant(expr)) {
					Errors_Throw(expr[0], "Only constant expressions are allowed in switch statement cases.");
				}
				let currentType = -1;
				let hadCollision = false;
				if (expr[1] == 22) {
					currentType = 1;
					hadCollision = intCollisions[expr[8]] !== undefined;
					intCollisions[expr[8]] = true;
				} else if (expr[1] == 28) {
					currentType = 2;
					hadCollision = strCollisions[expr[7]] !== undefined;
					strCollisions[expr[7]] = true;
				} else {
					Errors_Throw(expr[0], "Only integer and string constants are allowed to be used as switch statement cases.");
				}
				if (exprType == -1) {
					exprType = currentType;
				}
				if (exprType != currentType) {
					Errors_Throw(expr[0], "Switch statement cases must use the same type for all cases.");
				}
				if (hadCollision) {
					Errors_Throw(expr[0], "Switch statement contains multiple cases with the same value.");
				}
			}
			j += 1;
		}
		j = 0;
		while (j < chunk[2].length) {
			chunk[2][j] = StatementResolver_ResolveStatementSecondPass(resolver, chunk[2][j]);
			j += 1;
		}
		switch (chunk[2][chunk[2].length - 1][1]) {
			case 2:
				break;
			case 9:
				break;
			case 11:
				break;
			default:
				Errors_Throw(chunk[0][chunk[0].length - 1], "This switch statement case has a fall-through.");
				break;
		}
		i += 1;
	}
	return switchStmnt;
};

let StatementResolver_SecondPass_ThrowStatement = function(resolver, throwStmnt) {
	throwStmnt[2] = ExpressionResolver_ResolveExpressionSecondPass(resolver, throwStmnt[2]);
	if (IsExpressionConstant(throwStmnt[2])) {
		Errors_Throw(throwStmnt[2][0], "Only instances of Exception are throwable.");
	}
	return throwStmnt;
};

let StatementResolver_SecondPass_TryStatement = function(resolver, tryStmnt) {
	let oldBreakContext = resolver[9];
	resolver[9] = tryStmnt;
	StatementResolver_ResolveStatementArraySecondPass(resolver, tryStmnt[11]);
	let i = 0;
	while (i < tryStmnt[15].length) {
		let cc = tryStmnt[15][i];
		StatementResolver_ResolveStatementArraySecondPass(resolver, cc[3]);
		i += 1;
	}
	StatementResolver_ResolveStatementArraySecondPass(resolver, tryStmnt[13]);
	resolver[9] = oldBreakContext;
	return tryStmnt;
};

let StatementResolver_SecondPass_WhileLoop = function(resolver, whileLoop) {
	let oldBreakContext = resolver[9];
	resolver[9] = whileLoop;
	whileLoop[3] = ExpressionResolver_ResolveExpressionSecondPass(resolver, whileLoop[3]);
	StatementResolver_ResolveStatementArraySecondPass(resolver, whileLoop[11]);
	resolver[9] = oldBreakContext;
	return whileLoop;
};

let StaticContext_new = function() {
	return [TokenizerStaticContext_new(), {}, SpecialActionUtil_new(), StringSet_fromArray("public static".split(" "))];
};

let StringArraySlice = function(arr, skipStart, skipEnd) {
	let srcLen = arr.length;
	let dstLen = srcLen - skipStart - skipEnd;
	let output = PST$createNewArray(dstLen);
	let i = 0;
	while (i < dstLen) {
		output[i] = arr[skipStart + i];
		i += 1;
	}
	return output;
};

let StringArrayToList = function(arr) {
	let output = [];
	let i = 0;
	while (i < arr.length) {
		output.push(arr[i]);
		i += 1;
	}
	return output;
};

let StringSet_add = function(s, item) {
	s[0][item] = true;
	return s;
};

let StringSet_fromArray = function(items) {
	let s = StringSet_new();
	let i = 0;
	while (i < items.length) {
		s[0][items[i]] = true;
		i += 1;
	}
	return s;
};

let StringSet_fromList = function(items) {
	let s = StringSet_new();
	let i = 0;
	while (i < items.length) {
		s[0][items[i]] = true;
		i += 1;
	}
	return s;
};

let StringSet_has = function(s, item) {
	return s[0][item] !== undefined;
};

let StringSet_new = function() {
	return [{}];
};

let StringToUnicodeArray = function(strVal) {
	let bytes = PST$stringToUtf8Bytes(strVal);
	let output = PST$createNewArray(bytes.length);
	let length = bytes.length;
	let c = 0;
	let i = 0;
	let j = 0;
	while (i < length) {
		c = bytes[i];
		if (c < 128) {
			output[j] = c;
			i += 1;
		} else if ((c & 224) == 192) {
			output[j] = (c & 31) << 6 | (bytes[i + 1] & 63);
			i += 2;
		} else if ((c & 240) == 224) {
			output[j] = (c & 15) << 12 | (bytes[i + 1] & 63) << 6 | (bytes[i + 2] & 63);
			i += 3;
		} else if ((c & 240) == 224) {
			output[j] = (c & 7) << 18 | (bytes[i + 1] & 63) << 12 | (bytes[i + 2] & 63) << 6 | (bytes[i + 3] & 63);
			i += 4;
		} else {
			return null;
		}
		j += 1;
	}
	if (j < length) {
		let trimmedOutput = PST$createNewArray(j);
		while (j > 0) {
			j -= 1;
			trimmedOutput[j] = output[j];
		}
		output = trimmedOutput;
	}
	return output;
};

let SwitchChunk_new = function() {
	return [[], [], []];
};

let ThrowOpNotDefinedError = function(throwToken, op, left, right) {
	Errors_Throw(throwToken, ["The operation '", SimpleExprToTypeName(left), " ", op, " ", SimpleExprToTypeName(right), "' is not defined."].join(''));
};

let Token_getFingerprint = function(t) {
	if (t[5] == null) {
		t[5] = [t[1], ",", t[3] + '', ",", t[4] + ''].join('');
	}
	return t[5];
};

let Token_new = function(value, type, file, line, col) {
	return [value, file, type, line, col, null];
};

let Tokenize = function(file, content, ctx) {
	let contentPadded = content.split("\r\n").join("\n").split("\r").join("\n") + "\n\n\n\n\n";
	let chars = StringToUnicodeArray(contentPadded);
	let length = chars.length;
	let tokenizerCtx = ctx[0];
	let lines = PST$createNewArray(length);
	let cols = PST$createNewArray(length);
	let i = 0;
	let j = 0;
	let k = 0;
	let line = 1;
	let col = 1;
	let c = 0;
	i = 0;
	while (i < length) {
		c = chars[i];
		lines[i] = line;
		cols[i] = col;
		if (c == 10) {
			line += 1;
			col = 1;
		} else {
			col += 1;
		}
		i += 1;
	}
	let mode = 1;
	let tokenStart = 0;
	let tokenSubtype = 0;
	let tokenVal = "";
	let tokens = [];
	i = 0;
	while (i < length) {
		c = chars[i];
		switch (mode) {
			case 1:
				if (tokenizerCtx[2][c] !== undefined) {
				} else if (tokenizerCtx[0][c] !== undefined) {
					mode = 3;
					tokenStart = i;
				} else if (c == 34 || c == 39) {
					mode = 2;
					tokenStart = i;
					tokenSubtype = c;
				} else if (c == 47 && (chars[i + 1] == 47 || chars[i + 1] == 42)) {
					mode = 4;
					tokenSubtype = chars[i + 1];
					i += 1;
				} else {
					tokenVal = null;
					if (c == 62 && chars[i + 1] == c && chars[i + 2] == c && chars[i + 3] == 61) {
						tokenVal = ">>>=";
					} else if (tokenizerCtx[4][c] !== undefined) {
						let mcharCandidates = tokenizerCtx[4][c];
						j = 0;
						while (j < mcharCandidates.length && tokenVal == null) {
							let mcharCandidate = mcharCandidates[j];
							let isMatch = true;
							let mSize = mcharCandidate.length;
							k = 1;
							while (k < mSize) {
								if (mcharCandidate[k] != chars[i + k]) {
									isMatch = false;
									k += mSize;
								}
								k += 1;
							}
							if (isMatch) {
								tokenVal = UnicodeArrayToString(mcharCandidate);
							}
							j += 1;
						}
					}
					if (tokenVal == null) {
						tokenVal = String.fromCharCode(c) + "";
					}
					tokens.push(Token_new(tokenVal, 3, file, lines[i], cols[i]));
					i += tokenVal.length - 1;
				}
				break;
			case 3:
				if (!(tokenizerCtx[0][c] !== undefined)) {
					let tokenLen = i - tokenStart;
					tokenVal = UnicodeArrayToString_slice(chars, tokenStart, tokenLen);
					let firstChar = chars[tokenStart];
					let tokenType = 2;
					if (tokenizerCtx[1][firstChar] !== undefined) {
						if (firstChar == 48 && tokenLen > 2 && (chars[tokenStart + 1] == 120 || chars[tokenStart + 1] == 88)) {
							tokenType = 6;
						} else {
							tokenType = 5;
						}
					} else if (StringSet_has(tokenizerCtx[3], tokenVal)) {
						tokenType = 1;
					}
					tokens.push(Token_new(tokenVal, tokenType, file, lines[tokenStart], cols[tokenStart]));
					i -= 1;
					mode = 1;
				}
				break;
			case 4:
				if (c == 10 && tokenSubtype == 47) {
					mode = 1;
				} else if (c == 42 && tokenSubtype == c && chars[i + 1] == 47) {
					mode = 1;
					i += 1;
				}
				break;
			case 2:
				if (c == tokenSubtype) {
					tokenVal = UnicodeArrayToString_slice(chars, tokenStart, i - tokenStart + 1);
					mode = 1;
					tokens.push(Token_new(tokenVal, 4, file, lines[tokenStart], cols[tokenStart]));
				} else if (c == 92) {
					i += 1;
				}
				break;
			default:
				return null;
		}
		i += 1;
	}
	if (mode != 1) {
		if (mode == 2) {
			Errors_ThrowEof(file, "Unclosed string.");
		}
		Errors_ThrowEof(file, "Unclosed comment.");
	}
	i = 0;
	while (i < tokens.length) {
		let current = tokens[i];
		if (current != null) {
			let left = null;
			let right = null;
			if (i > 0) {
				left = tokens[i - 1];
			}
			if (i + 1 < tokens.length) {
				right = tokens[i + 1];
			}
			if (left != null && (left[3] != current[3] || left[4] + left[0].length != current[4])) {
				left = null;
			}
			if (right != null && (right[3] != current[3] || current[4] + current[0].length != right[4])) {
				right = null;
			}
			if (current[0] == "@" && right != null && (right[2] == 2 || right[2] == 1)) {
				current[0] += right[0];
				current[2] = 8;
				tokens[i + 1] = null;
			} else if (current[0] == ".") {
				if (left != null && left[2] == 5) {
					left[0] += ".";
					left[2] = 7;
					tokens[i] = left;
					tokens[i - 1] = null;
					current = left;
					left = null;
				}
				if (right != null && right[2] == 5) {
					current[0] += right[0];
					tokens[i + 1] = null;
					current[2] = 7;
				}
			}
		}
		i += 1;
	}
	let output = [];
	i = 0;
	while (i < tokens.length) {
		if (tokens[i] != null) {
			output.push(tokens[i]);
		}
		i += 1;
	}
	return [...(output)];
};

let TokenizerStaticContext_new = function() {
	let ctx = [{}, {}, {}, null, {}];
	let i = 0;
	i = 0;
	while (i < 10) {
		ctx[1][48 + i] = true;
		ctx[0][48 + i] = true;
		i += 1;
	}
	i = 0;
	while (i < 26) {
		ctx[0][65 + i] = true;
		ctx[0][97 + i] = true;
		i += 1;
	}
	ctx[0][95] = true;
	let ws = " \r\n\t";
	i = 0;
	while (i < ws.length) {
		ctx[2][ws.charAt(i).charCodeAt(0)] = true;
		i += 1;
	}
	ctx[3] = StringSet_fromArray("function class field property constructor const enum\nbase this\nnull false true new\nis typeof\nif else for while do break continue switch case default yield return\nthrow try catch finally\nimport namespace\npublic static readonly abstract".trim().split("\n").join(" ").split(" "));
	let mcharTokens = ">>>= >>> <<= >>= **= ++ -- && || ** == != <= >= => -> << >> ?? += -= *= %= /= |= &= ^=".split(" ");
	i = 0;
	while (i < mcharTokens.length) {
		let mcharTok = mcharTokens[i];
		let uchars = PST$createNewArray(mcharTok.length);
		let j = 0;
		while (j < uchars.length) {
			uchars[j] = mcharTok.charAt(j).charCodeAt(0);
			j += 1;
		}
		let firstChar = uchars[0];
		if (!(ctx[4][firstChar] !== undefined)) {
			ctx[4][firstChar] = [];
		}
		ctx[4][firstChar].push(uchars);
		i += 1;
	}
	return ctx;
};

let Tokens_doesNextInclude5 = function(tokens, val1, val2, val3, val4, val5) {
	let next = Tokens_peekValue(tokens);
	return next == val1 || next == val2 || next == val3 || next == val4 || next == val5;
};

let Tokens_doesNextInclulde2 = function(tokens, val1, val2) {
	return Tokens_doesNextInclulde4(tokens, val1, val2, null, null);
};

let Tokens_doesNextInclulde3 = function(tokens, val1, val2, val3) {
	return Tokens_doesNextInclulde4(tokens, val1, val2, val3, null);
};

let Tokens_doesNextInclulde4 = function(tokens, val1, val2, val3, val4) {
	let next = Tokens_peekValue(tokens);
	return next == val1 || next == val2 || next == val3 || next == val4;
};

let Tokens_ensureMore = function(tokens) {
	if (!Tokens_hasMore(tokens)) {
		Errors_ThrowEof(tokens[3], "Unexpected end of file");
	}
};

let Tokens_hasMore = function(tokens) {
	return tokens[0] < tokens[1];
};

let Tokens_isNext = function(tokens, value) {
	return Tokens_peekValue(tokens) == value;
};

let Tokens_isSequenceNext2 = function(tokens, val1, val2) {
	return Tokens_isSequenceNext4(tokens, val1, val2, null, null);
};

let Tokens_isSequenceNext3 = function(tokens, val1, val2, val3) {
	return Tokens_isSequenceNext4(tokens, val1, val2, val3, null);
};

let Tokens_isSequenceNext4 = function(tokens, val1, val2, val3, val4) {
	if (val1 != null && tokens[0] < tokens[1] && tokens[2][tokens[0]][0] != val1) {
		return false;
	}
	if (val2 != null && tokens[0] + 1 < tokens[1] && tokens[2][tokens[0] + 1][0] != val2) {
		return false;
	}
	if (val3 != null && tokens[0] + 2 < tokens[1] && tokens[2][tokens[0] + 2][0] != val3) {
		return false;
	}
	if (val4 != null && tokens[0] + 3 < tokens[1] && tokens[2][tokens[0] + 3][0] != val4) {
		return false;
	}
	return true;
};

let Tokens_peek = function(tokens) {
	if (tokens[0] >= tokens[1]) {
		return null;
	}
	return tokens[2][tokens[0]];
};

let Tokens_peekAhead = function(tokens, distance) {
	if (tokens[0] + distance < tokens[1]) {
		return tokens[2][tokens[0] + distance];
	}
	return null;
};

let Tokens_peekType = function(tokens) {
	if (!Tokens_hasMore(tokens)) {
		return 9;
	}
	return Tokens_peek(tokens)[2];
};

let Tokens_peekValue = function(tokens) {
	let t = Tokens_peek(tokens);
	if (t == null) {
		return null;
	}
	return t[0];
};

let Tokens_peekValueNonNull = function(tokens) {
	let v = Tokens_peekValue(tokens);
	if (v == null) {
		return "";
	}
	return v;
};

let Tokens_pop = function(tokens) {
	if (tokens[0] >= tokens[1]) {
		return null;
	}
	let t = tokens[2][tokens[0]];
	tokens[0] += 1;
	return t;
};

let Tokens_popExpected = function(tokens, value) {
	let output = Tokens_pop(tokens);
	if (output == null) {
		Tokens_ensureMore(tokens);
	}
	if (output[0] != value) {
		Errors_Throw(output, ["Expected '", value, "' but found '", output[0], "' instead."].join(''));
	}
	if (output[2] == 1) {
		fail("Use popKeyword instead.");
	}
	return output;
};

let Tokens_popIfPresent = function(tokens, value) {
	if (Tokens_peekValue(tokens) == value) {
		tokens[0] += 1;
		return true;
	}
	return false;
};

let Tokens_popKeyword = function(tokens, value) {
	let next = Tokens_pop(tokens);
	if (next == null) {
		Tokens_ensureMore(tokens);
	}
	if (next[0] != value || next[2] != 1) {
		Errors_Throw(next, ["Expected '", value, "' keyword but found '", next[0], "' instead."].join(''));
	}
	return next;
};

let Tokens_popName = function(tokens, purposeForErrorMessage) {
	let t = Tokens_pop(tokens);
	if (t == null) {
		Tokens_ensureMore(tokens);
	}
	if (t[2] != 2) {
		Errors_Throw(t, ["Expected ", purposeForErrorMessage, " but found '", t[0], "' instead."].join(''));
	}
	return t;
};

let TokenStream_new = function(file, tokens) {
	return [0, tokens.length, tokens, file];
};

let TryDoExactLookupForConstantEntity = function(resolver, file, fqNamespace, dottedEntityName) {
	if (resolver[5][dottedEntityName] !== undefined) {
		return resolver[5][dottedEntityName];
	}
	let nsParts = StringArrayToList(fqNamespace.split("."));
	while (nsParts.length > 0) {
		let lookupName = [nsParts.join("."), ".", dottedEntityName].join('');
		if (resolver[5][lookupName] !== undefined) {
			return resolver[5][lookupName];
		}
		nsParts.splice(nsParts.length - 1, 1);
	}
	let entityNameSegments = dottedEntityName.split(".");
	if (file[5][entityNameSegments[0]] !== undefined) {
		let targetModule = file[5][entityNameSegments[0]][5];
		let scopedName = StringArraySlice(entityNameSegments, 1, 0).join(".");
		return CompiledModuleEntityLookup(targetModule, scopedName);
	}
	let i = 0;
	while (i < file[4].length) {
		let imp = file[4][i];
		if (imp[3] == null) {
			return CompiledModuleEntityLookup(imp[5], dottedEntityName);
		}
		i += 1;
	}
	return null;
};

let TryParseFloat = function(throwToken, rawValue) {
	let o = PST$createNewArray(2);
	PST$floatParseHelper(o, rawValue);
	if (o[0] > 0) {
		return o[1];
	}
	Errors_Throw(throwToken, "Invalid float constant");
	return 0.0;
};

let TryParseInteger = function(throwToken, rawValue, isHex) {
	let output = 0;
	let start = 0;
	let baseMultiplier = 10;
	let chars = PST$stringToUtf8Bytes(rawValue.toLowerCase());
	if (isHex) {
		start = 2;
		baseMultiplier = 16;
	}
	let i = start;
	while (i < chars.length) {
		let d = chars[i];
		let digitVal = 0;
		if (d >= 48 && d <= 57) {
			digitVal = d - 48;
		} else if (isHex && d >= 97 && d <= 102) {
			digitVal = d - 97 + 10;
		} else {
			if (isHex) {
				Errors_Throw(throwToken, "Invalid hexadecimal constant.");
			}
			Errors_Throw(throwToken, "Invalid integer constant");
		}
		output = output * baseMultiplier + digitVal;
		i++;
	}
	return output;
};

let TryParseString = function(throwToken, rawValue) {
	let output = [];
	let length = rawValue.length - 1;
	let c = "";
	let i = 1;
	while (i < length) {
		c = rawValue.substring(i, i + 1);
		if (c == "\\") {
			i += 1;
			if (i == length) {
				Errors_Throw(throwToken, "Invalid backslash in string constant.");
			}
			c = rawValue.substring(i, i + 1);
			if (c == "n") {
				c = "\n";
			} else if (c == "r") {
				c = "\r";
			} else if (c == "'" || c == "\"" || c == "\\") {
			} else if (c == "t") {
				c = "\t";
			} else {
				Errors_Throw(throwToken, ["Unrecognized string escape sequence: '\\", c, "'"].join(''));
			}
		}
		output.push(c);
		i += 1;
	}
	return output.join("");
};

let TryPopAssignmentOp = function(tokens) {
	let op = Tokens_peekValue(tokens);
	if (op == null) {
		return null;
	}
	let isOp = false;
	switch (op.charAt(0)) {
		case "=":
			if (op == "=") {
				isOp = true;
			}
			break;
		case "+":
			if (op == "+=") {
				isOp = true;
			}
			break;
		case "-":
			if (op == "-=") {
				isOp = true;
			}
			break;
		case "*":
			if (op == "*=") {
				isOp = true;
			}
			if (op == "**=") {
				isOp = true;
			}
			break;
		case "/":
			if (op == "/=") {
				isOp = true;
			}
			break;
		case "%":
			if (op == "%=") {
				isOp = true;
			}
			break;
		case "<":
			if (op == "<<=") {
				isOp = true;
			}
			break;
		case ">":
			if (op == ">>=") {
				isOp = true;
			}
			if (op == ">>>=") {
				isOp = true;
			}
			break;
		case "&":
			if (op == "&=") {
				isOp = true;
			}
			break;
		case "|":
			if (op == "|=") {
				isOp = true;
			}
			break;
		case "^":
			if (op == "^=") {
				isOp = true;
			}
			break;
	}
	if (isOp) {
		return Tokens_pop(tokens);
	}
	return null;
};

let UnicodeArrayToString = function(chars) {
	return UnicodeArrayToString_slice(chars, 0, chars.length);
};

let UnicodeArrayToString_slice = function(chars, start, length) {
	let sb = [];
	let end = start + length;
	let i = start;
	while (i < end) {
		sb.push(String.fromCharCode(chars[i]));
		i += 1;
	}
	return sb.join("");
};
return [PST$registerExtensibleCallback, _Errors_ThrowImpl, AbstractEntity_new, AddImplicitIncrementingEnumValueDefinitions, allocateStringAndTokenIds, AttachEntityToParseTree, bsbFlatten, bsbFrom4Bytes, bsbFromBytes, bsbFromInt, bsbFromLenString, bsbFromUtf8String, bsbJoin2, bsbJoin3, bsbJoin4, bsbJoin5, bsbJoin8, BuildFakeDotChain, bundleClass, BundleClassInfo_new, bundleCompilation, bundleEntity, bundleEnum, BundleEnumInfo_createFromEntity, bundleFunction, BundleFunctionInfo_new, ByteCodeBuffer_from2, ByteCodeBuffer_fromRow, ByteCodeRow_new, ByteCodeUtil_ensureBooleanExpression, ByteCodeUtil_ensureIntegerExpression, CatchChunk_new, ClassEntity_new, ClassSorter_calcDepth, ClassSorter_SortClassesInDeterministicDependencyOrder, CompilationBundle_new, CompiledModule_AddLambdas, CompiledModule_InitializeLookups, CompiledModule_new, CompiledModuleEntityLookup, CompilerContext_CalculateCompilationOrder, CompilerContext_CompileModule, CompilerContext_new, ConstEntity_new, convertToBuffer, create0, create1, create2, create3, createFakeToken, createFakeTokenFromTemplate, DotField_getVariableRootedDottedChain, Entity_getMemberLookup, EntityParser_ClassifyToken, EntityResolver_ConvertFieldDefaultValueIntoSetter, EntityResolver_DetermineMemberOffsets, EntityResolver_GetNextAutoVarId, EntityResolver_ResetAutoVarId, EntityResolver_ResolveFunctionFirstPass, EntityResolver_ResolveFunctionSecondPass, EnumEntity_new, Errors_Throw, Errors_ThrowEof, Errors_ThrowGeneralError, Errors_ThrowNotImplemented, ExportUtil_exportBundle, ExportUtil_exportCode, Expression_cloneWithNewToken, Expression_createBaseCtorReference, Expression_createBaseReference, Expression_createBinaryOp, Expression_createBoolConstant, Expression_createBracketIndex, Expression_createClassReference, Expression_createConstructorInvocation, Expression_createConstructorReference, Expression_createDictionaryDefinition, Expression_createDotField, Expression_createEnumConstant, Expression_createEnumReference, Expression_createExtensionInvocation, Expression_createExtensionReference, Expression_createFloatConstant, Expression_createFunctionInvocation, Expression_createFunctionReference, Expression_createImportReference, Expression_createInlineIncrement, Expression_createIntegerConstant, Expression_createLambda, Expression_createListDefinition, Expression_createNamespaceReference, Expression_createNegatePrefix, Expression_createNullConstant, Expression_createSliceExpression, Expression_createStringConstant, Expression_createTernary, Expression_createThisReference, Expression_createTypeof, Expression_createVariable, Expression_new, ExpressionResolver_FindLocallyReferencedEntity, ExpressionResolver_FirstPass_BaseCtorReference, ExpressionResolver_FirstPass_BinaryOp, ExpressionResolver_FirstPass_BitwiseNot, ExpressionResolver_FirstPass_BoolConst, ExpressionResolver_FirstPass_BoolNot, ExpressionResolver_FirstPass_ConstructorInvocation, ExpressionResolver_FirstPass_ConstructorReference, ExpressionResolver_FirstPass_DictionaryDefinition, ExpressionResolver_FirstPass_DotField, ExpressionResolver_FirstPass_FloatConstant, ExpressionResolver_FirstPass_FunctionInvocation, ExpressionResolver_FirstPass_Index, ExpressionResolver_FirstPass_InlineIncrement, ExpressionResolver_FirstPass_IntegerConstant, ExpressionResolver_FirstPass_Lambda, ExpressionResolver_FirstPass_ListDefinition, ExpressionResolver_FirstPass_NegativeSign, ExpressionResolver_FirstPass_NullConst, ExpressionResolver_FirstPass_Slice, ExpressionResolver_FirstPass_StringConstant, ExpressionResolver_FirstPass_Ternary, ExpressionResolver_FirstPass_This, ExpressionResolver_FirstPass_TypeOf, ExpressionResolver_FirstPass_Variable, ExpressionResolver_IntegerRequired, ExpressionResolver_ResolveExpressionArrayFirstPass, ExpressionResolver_ResolveExpressionArraySecondPass, ExpressionResolver_ResolveExpressionFirstPass, ExpressionResolver_ResolveExpressionSecondPass, ExpressionResolver_SecondPass_BaseCtorReference, ExpressionResolver_SecondPass_BinaryOp, ExpressionResolver_SecondPass_BitwiseNot, ExpressionResolver_SecondPass_BoolConst, ExpressionResolver_SecondPass_BoolNot, ExpressionResolver_SecondPass_ClassReference, ExpressionResolver_SecondPass_ConstructorReference, ExpressionResolver_SecondPass_DictionaryDefinition, ExpressionResolver_SecondPass_DotField, ExpressionResolver_SecondPass_EnumConstant, ExpressionResolver_SecondPass_ExtensionInvocation, ExpressionResolver_SecondPass_FloatConstant, ExpressionResolver_SecondPass_FunctionInvocation, ExpressionResolver_SecondPass_FunctionReference, ExpressionResolver_SecondPass_ImportReference, ExpressionResolver_SecondPass_Index, ExpressionResolver_SecondPass_InlineIncrement, ExpressionResolver_SecondPass_IntegerConstant, ExpressionResolver_SecondPass_Lambda, ExpressionResolver_SecondPass_ListDefinition, ExpressionResolver_SecondPass_NamespaceReference, ExpressionResolver_SecondPass_NegativeSign, ExpressionResolver_SecondPass_NullConstant, ExpressionResolver_SecondPass_Slice, ExpressionResolver_SecondPass_StringConstant, ExpressionResolver_SecondPass_Ternary, ExpressionResolver_SecondPass_ThisConstant, ExpressionResolver_SecondPass_TypeOf, ExpressionResolver_SecondPass_Variable, ExpressionResolver_WrapEntityIntoReferenceExpression, fail, FieldEntity_new, FileContext_initializeImportLookup, FileContext_new, finalizeBreakContinue, flatten, FlattenBinaryOpChain, FlattenEntities, FloatToStringWorkaround, FunctionEntity_BuildConstructor, FunctionEntity_BuildLambda, FunctionEntity_BuildMethodOrStandalone, FunctionEntity_new, GEN_BUILTINS_base64, GEN_BUILTINS_builtins, GEN_BUILTINS_json, GEN_BUILTINS_math, GEN_BUILTINS_random, GEN_BUILTINS_textencoding, GEN_BUILTINS_xml, GetBuiltinRawStoredString, getDeterministOrderOfModules, GetNumericValueOfConstantExpression, GetSourceForBuiltinModule, GetStringFromConstantExpression, ImportParser_AdvanceThroughImports, ImportParser_createBuiltinImport, ImportStatement_new, IsBuiltInModule, IsExpressionConstant, IsExpressionNumericConstant, join2, join3, join4, join5, join6, join7, LookupUtil_DoFirstPassVariableLookupThroughImports, LookupUtil_DoLookupForName, LookupUtil_tryCreateModuleMemberReference, ModuleWrapperEntity_new, NamespaceEntity_new, OrderStringsByDescendingFrequencyUsingLookup, PadIntegerToSize, ParseAddition, ParseAnnotations, ParseAnyForLoop, ParseArgDefinitionList, ParseAtomicExpression, ParseBitshift, ParseBitwise, ParseBooleanCombination, ParseBreakContinue, ParseClass, ParseCodeBlock, ParseCodeBlockList, ParseConst, ParseConstructor, ParseDictionaryDefinition, ParseDoWhileLoop, ParseEnum, ParseEquality, ParseExponent, ParseExpression, ParseField, ParseForEachLoop, ParseFunctionDefinition, ParseIfStatement, ParseInequality, ParseInlineIncrementPrefix, ParseLambda, ParseListDefinition, ParseMultiplication, ParseNamespace, ParseNegatePrefix, ParseNullCoalesce, ParseOutEntities, ParseReturn, ParseStatement, ParseSwitch, ParseTernary, ParseThrow, ParseTraditionalForLoop, ParseTry, ParseTypeofPrefix, ParseUnaryPrefix, ParseUnarySuffix, ParseWhileLoop, PerformFullResolutionPassOnConstAndEnums, PUBLIC_CompleteCompilation, PUBLIC_EnsureDependenciesFulfilled, PUBLIC_GetNextRequiredModuleId, PUBLIC_getTokenErrPrefix, PUBLIC_SupplyFilesForModule, Resolve, ResolveBaseClassesAndEstablishClassOrder, Resolver_DetermineConstAndEnumResolutionOrder, Resolver_GetEnumMemberIndex, Resolver_GetListOfUnresolvedConstReferences, Resolver_GetListOfUnresolvedConstReferencesImpl, Resolver_isValidRegisteredExtension, Resolver_new, Resolver_ReportNewLambda, serializeAssignField, serializeAssignIndex, serializeAssignVariable, serializeBaseCtorReference, serializeBinaryOp, serializeBitwiseNot, serializeBoolConst, serializeBooleanNot, serializeBreak, serializeClassReference, serializeCodeBlock, serializeConstructorInvocation, serializeContinue, serializeDictionaryDefinition, serializeDotField, serializeDoWhileLoop, serializeExpression, serializeExpressionStatement, serializeExtensionInvocation, serializeFloatConstant, serializeForEachLoop, serializeForLoop, serializeFunctionInvocation, serializeFunctionReference, serializeIfStatement, serializeIndex, serializeInlineIncrement, serializeInlineIncrementDotField, serializeInlineIncrementIndex, serializeInlineIncrementVar, serializeIntegerConstant, serializeLambda, serializeListDefinition, serializeNegativeSign, serializeNullConstant, serializeReturn, serializeSlice, serializeSpecialAction, serializeStatement, serializeStringConstant, serializeSwitchStatement, serializeTernary, serializeThis, serializeThrowStatement, serializeTryStatement, serializeTypeOf, serializeVariable, serializeWhileLoop, SimpleExprToTypeName, SpecialActionUtil_GetSpecialActionArgc, SpecialActionUtil_GetSpecialActionOpCode, SpecialActionUtil_IsSpecialActionAndNotExtension, SpecialActionUtil_new, Statement_createAssignment, Statement_createBreakContinue, Statement_createDoWhile, Statement_createExpressionAsStatement, Statement_createForEachLoop, Statement_createForLoop, Statement_createIfStatement, Statement_createReturn, Statement_createSwitchStatement, Statement_createThrow, Statement_createTry, Statement_createWhileLoop, Statement_new, StatementParser_IdentifyKeywordType, StatementResolver_FirstPass_Assignment, StatementResolver_FirstPass_Break, StatementResolver_FirstPass_Continue, StatementResolver_FirstPass_DoWhileLoop, StatementResolver_FirstPass_ForEachLoop, StatementResolver_FirstPass_ForLoop, StatementResolver_FirstPass_IfStatement, StatementResolver_FirstPass_SwitchStatement, StatementResolver_FirstPass_Try, StatementResolver_FirstPass_WhileLoop, StatementResolver_ResolveStatementArrayFirstPass, StatementResolver_ResolveStatementArraySecondPass, StatementResolver_ResolveStatementFirstPass, StatementResolver_ResolveStatementSecondPass, StatementResolver_SecondPass_Assignment, StatementResolver_SecondPass_Break, StatementResolver_SecondPass_Continue, StatementResolver_SecondPass_DoWhileLoop, StatementResolver_SecondPass_ExpressionAsStatement, StatementResolver_SecondPass_ForEachLoop, StatementResolver_SecondPass_ForLoop, StatementResolver_SecondPass_IfStatement, StatementResolver_SecondPass_Return, StatementResolver_SecondPass_SwitchStatement, StatementResolver_SecondPass_ThrowStatement, StatementResolver_SecondPass_TryStatement, StatementResolver_SecondPass_WhileLoop, StaticContext_new, StringArraySlice, StringArrayToList, StringSet_add, StringSet_fromArray, StringSet_fromList, StringSet_has, StringSet_new, StringToUnicodeArray, SwitchChunk_new, ThrowOpNotDefinedError, Token_getFingerprint, Token_new, Tokenize, TokenizerStaticContext_new, Tokens_doesNextInclude5, Tokens_doesNextInclulde2, Tokens_doesNextInclulde3, Tokens_doesNextInclulde4, Tokens_ensureMore, Tokens_hasMore, Tokens_isNext, Tokens_isSequenceNext2, Tokens_isSequenceNext3, Tokens_isSequenceNext4, Tokens_peek, Tokens_peekAhead, Tokens_peekType, Tokens_peekValue, Tokens_peekValueNonNull, Tokens_pop, Tokens_popExpected, Tokens_popIfPresent, Tokens_popKeyword, Tokens_popName, TokenStream_new, TryDoExactLookupForConstantEntity, TryParseFloat, TryParseInteger, TryParseString, TryPopAssignmentOp, UnicodeArrayToString, UnicodeArrayToString_slice];
})();

    return {
      CompilerContext_new,
      PUBLIC_GetNextRequiredModuleId,
      PUBLIC_SupplyFilesForModule,
      PUBLIC_EnsureDependenciesFulfilled,
      PUBLIC_CompleteCompilation,
      PUBLIC_getTokenErrPrefix,
      registerExt: PASTEL_regCallback,
    };
  })();

  const IS_DEBUG = true;

  PST.registerExt('throwParserException', (args) => {
    // TODO: this should just receive a string instead of making a distinction here to assemble the error message.
    let [n, a, b] = args;
    if (n === 1) throw new Error(PST.PUBLIC_getTokenErrPrefix(a) + b);
    if (n === 2) throw new Error(`[${a}] ${b}`);
    if (n === 3) throw new Error(a)
    throw new Error();
  });

  let newCompilationEngine = (languageId, version, extensions) => {

    let extensionNames = [...extensions].map(v => '' + v);

    let createAdaptiveCompilation = (rootModuleId) => {

      // TODO: include error logic and catching in provideFiles and shortcircuit getCompilation result when applicable.
      let compiler = PST.CompilerContext_new(rootModuleId + '', languageId + '', version + '', [...extensionNames]);
      let nextModuleIdCache = null;
      let isDone = false;

      let updateNext = () => {
        nextModuleIdCache = PST.PUBLIC_GetNextRequiredModuleId(compiler);
        isDone = nextModuleIdCache === null;
      };
      updateNext();

      // TODO: why is isBuiltIn ignored?
      let provideFilesImpl = (nextModId, filesLookup, isBuiltIn) => {
        if (nextModId !== nextModuleIdCache) throw new Error('');
        PST.PUBLIC_SupplyFilesForModule(compiler, nextModId, { ...filesLookup }, false, false);
        updateNext();
      };

      let getCompilation = () => {
        if (!isDone) throw new Error('');
        PST.PUBLIC_EnsureDependenciesFulfilled(compiler);
        let bytes = PST.PUBLIC_CompleteCompilation(compiler);
        return { byteCodePayload: bytes };
      };

      return Object.freeze({
        isComplete: () => isDone,
        getNextRequiredModule: () => nextModuleIdCache,
        provideFilesForUserModuleCompilation: (nextModId, filesLookup) => provideFilesImpl(nextModId, filesLookup, false),
        provideFilesForBuiltinLibraryModuleCompilation: (nextModId, filesLookup) => provideFilesImpl(nextModId, filesLookup, true),
        getCompilation: () => {
          if (IS_DEBUG) return getCompilation();
          try {
            return getCompilation();
          } catch (ex) {
            return { errorMessage: ex.message };
          }
        },
      });
    };

    let doStaticCompilation = (moduleId, userCodeFilesByModuleId, builtinCodeFilesByModuleId) => {
      let adcomp = createAdaptiveCompilation(moduleId);
      let userCode = userCodeFilesByModuleId || {};
      let builtinCode = builtinCodeFilesByModuleId || {};
      while (!adcomp.isComplete()) {
        let nextModId = adcomp.getNextRequiredModule();
        let sources = [userCode, builtinCode];
        let moduleFound = false;
        for (let s = 0; s < 2; s++) {
          let isUserCode = s === 0;
          let files = sources[s][nextModId];
          if (files) {
            moduleFound = true;
            if (IS_DEBUG) {
              if (isUserCode) adcomp.provideFilesForUserModuleCompilation(nextModId, files);
              else adcomp.provideFilesForBuiltinLibraryModuleCompilation(nextModId, files);
            } else {
              try {
                if (isUserCode) adcomp.provideFilesForUserModuleCompilation(nextModId, files);
                else adcomp.provideFilesForBuiltinLibraryModuleCompilation(nextModId, files);
              } catch (ex) {
                return { errorMessage: ex.message };
              }
            }
          }
        }
      }
      let result = adcomp.getCompilation();
      return result;
    };

    return Object.freeze({
      createAdaptiveCompilation,
      doStaticCompilation,
    });
  };
  return newCompilationEngine;
})();

PlexiOS.HtmlUtil.registerComponent('CommonScript_compile_0_1_0', () => CommonScriptCompiler);
})();
