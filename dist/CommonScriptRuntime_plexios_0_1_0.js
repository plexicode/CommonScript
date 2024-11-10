(() => {
  const CommonScript = (() => {

  //
const [bubbleException, buildAsciiStringImpl, buildBase64String, buildFloat, buildFunctionFromInfo, buildInteger, buildList, buildString, convertToStringImpl, createClassInfo, createMainTask, createNewTask, createStringFromUnicodeArray, DictImpl_ensureCapacity, exceptionCatcherRouteException, ExRes_Done, ExRes_HardCrash, ExRes_Suspend, finalizeExecutionContext, FunctionPointer_cloneWithNewType, generateNameLookup, generateStackTrace, generateTryDescriptors, getExceptionMessage, getGlobalsFromTask, increaseValueStackCapacity, injectNameLookup, isValueEqual, List_add, List_expandCapacity, List_get, List_join, List_pop, List_removeAt, List_set, new_ByteCodeRow, new_ExecutionContext, new_ExecutionResult, new_GlobalValues, ParseRaw_entitiesSection_classMemberResolver, ParseRaw_entitiesSection_parseClasses, ParseRaw_entitiesSection_parseEnums, ParseRaw_entitiesSection_parseFunctions, ParseRaw_parseEntityData, ParseRaw_parseMetadata, ParseRaw_parseStringData, ParseRaw_parseTokenData, ParseRaw_popByteCodeRows, ParseRaw_popBytes, ParseRaw_popFixedLenString, ParseRaw_popInt, ParseRaw_popLenString, ParseRaw_popSingleByte, ParseRawData, PUBLIC_getApplicationContextFromTask, PUBLIC_getExecutionContextError, PUBLIC_getTaskResultError, PUBLIC_getTaskResultSleepAmount, PUBLIC_getTaskResultStatus, PUBLIC_initializeExecutionContext, PUBLIC_listValueAdd, PUBLIC_requestTaskSuspension, PUBLIC_startMainTask, PUBLIC_unwrapInteger, PUBLIC_unwrapNativeHandle, PUBLIC_valueToString, PUBLIC_wrapBoolean, PUBLIC_wrapInteger, PUBLIC_wrapNativeHandle, PUBLIC_wrapString, RunInterpreter, RunInterpreterImpl, Sort_buildTaskList, Sort_end, Sort_getNextCmp, Sort_proceedWithCmpResult, Sort_start, stringFlatten, stringUtil_changeCase, stringUtil_split, stringUtil_trim, ThrowError, ThrowErrorImpl, tryGetNameId, valueArrayIncreaseCapacity, valueToHumanString] = (() => {
let PST$stringToUtf8Bytes = s => Array.from(new TextEncoder().encode(s));

let PST$createNewArray = s => {
	let o = [];
	while (s --> 0) o.push(null);
	return o;
};

let PST$extCallbacks = {};

let PST$registerExtensibleCallback = (name, fn) => { PST$extCallbacks[name] = fn; };

let bubbleException = function(task, exceptionValue) {
	let frame = task[2];
	let ec = task[1];
	let keepGoing = true;
	while (keepGoing) {
		if (frame == null) {
			task[2] = null;
			return true;
		}
		let pc = frame[1];
		let td = ec[10][pc];
		if (td != null) {
			keepGoing = false;
			if (pc < td[1]) {
				frame[1] = td[1] - 1;
				frame[9] = exceptionValue;
			} else if (pc >= td[2]) {
				frame[1] = td[3] - 1;
				frame[9] = exceptionValue;
				frame[10] = true;
			} else {
				frame[1] = td[2];
				frame[9] = exceptionValue;
				frame[10] = true;
			}
		} else {
			frame = frame[0];
		}
	}
	task[2] = frame;
	frame[4] = frame[5];
	return false;
};

let buildAsciiStringImpl = function(rawValue) {
	return [rawValue.length, false, PST$stringToUtf8Bytes(rawValue), rawValue, null, null];
};

let buildBase64String = function(rawBytes) {
	let pairs = [];
	let sz = rawBytes.length;
	let n = 0;
	let i = 0;
	while (i < sz) {
		n = rawBytes[i];
		pairs.push((n >> 6) & 3);
		pairs.push((n >> 4) & 3);
		pairs.push((n >> 2) & 3);
		pairs.push(n & 3);
		i += 1;
	}
	let chars = PST$createNewArray(64);
	let j = 0;
	while (j < 26) {
		chars[j] = String.fromCharCode("A".charCodeAt(0) + j);
		chars[j + 26] = String.fromCharCode("a".charCodeAt(0) + j);
		if (j < 10) {
			chars[j + 52] = String.fromCharCode("0".charCodeAt(0) + j);
		}
		j += 1;
	}
	chars[62] = "+";
	chars[63] = "/";
	let pairSize = pairs.length;
	pairs.push(0);
	pairs.push(0);
	let sb = [];
	let k = 0;
	while (k < pairSize) {
		n = (pairs[k] << 4) | (pairs[k + 1] << 2) | pairs[k + 2];
		sb.push(chars[n]);
		k += 3;
	}
	while (sb.length % 4 != 0) {
		sb.push("=");
	}
	return sb.join("");
};

let buildFloat = function(v) {
	return [4, v];
};

let buildFunctionFromInfo = function(fn) {
	let fp = [1, fn[0], fn[1], fn[2], fn, null];
	return [11, fp];
};

let buildInteger = function(g, value) {
	if (value < 0) {
		if (value > -1200) {
			return g[4][-value];
		}
	} else if (value < 1200) {
		return g[3][value];
	}
	return [3, value];
};

let buildList = function(ec, values, copyValues, lengthOrNegativeOne) {
	let id = ec[17];
	ec[17] += 1;
	let size = lengthOrNegativeOne;
	if (size == -1) {
		size = values.length;
	}
	if (copyValues) {
		let buf = PST$createNewArray(size);
		let i = 0;
		while (i < size) {
			buf[i] = values[i];
			i += 1;
		}
		values = buf;
	}
	return [9, [id, size, size, values]];
};

let buildString = function(g, rawValue, isCommon) {
	if (g[6][rawValue] !== undefined) {
		return g[6][rawValue];
	}
	let charValues = PST$stringToUtf8Bytes(rawValue);
	let s = [charValues.length, false, charValues, rawValue, null, null];
	let v = [5, s];
	if (isCommon) {
		g[6][rawValue] = v;
	}
	return v;
};

let convertToStringImpl = function(g, v) {
	switch (v[0]) {
		case 1:
			return buildString(g, "null", true)[1];
		case 2:
			if (v[1]) {
				return buildString(g, "true", true)[1];
			}
			return buildString(g, "false", true)[1];
		case 3:
			let n = v[1];
			if (n < 20 && n > -20) {
				return buildString(g, n + '', true)[1];
			}
			return buildAsciiStringImpl(n + '');
		case 4:
			return buildString(g, valueToHumanString(v), false)[1];
		case 5:
			return v[1];
		default:
			break;
	}
	return buildAsciiStringImpl("TODO: string for this type");
};

let createClassInfo = function(id, parentId, name, ctor, newFields, newMemberInfoFlags, cctorIdOrZero) {
	let ctorVal = [11, [5, ctor[0], ctor[1], ctor[2], ctor, null]];
	let fieldCount = newFields.length;
	let info = [id, parentId, null, name, ctorVal, cctorIdOrZero, false, {}, {}, PST$createNewArray(fieldCount), PST$createNewArray(fieldCount), newFields, newMemberInfoFlags, null, {}, {}];
	info[13] = [13, info];
	return info;
};

let createMainTask = function(ec, cliArgs) {
	if (ec[16] != 1) {
		return null;
	}
	finalizeExecutionContext(ec);
	let mainFn = ec[5][ec[3]["main"]];
	let args = ec[1][0];
	let argList = PST$createNewArray(1);
	return createNewTask(ec, mainFn, argList);
};

let createNewTask = function(ec, fpValue, args) {
	if (fpValue[0] != 11) {
		return null;
	}
	let fp = fpValue[1];
	let pc = 0;
	let argc = args.length;
	let argcMax = argc;
	switch (fp[0]) {
		case 1:
			pc = fp[3];
			if (argc < fp[1] || argc > fp[2]) {
				return null;
			}
			argcMax = fp[2];
			break;
		default:
			return null;
	}
	let argsClone = PST$createNewArray(argc);
	let i = 0;
	while (i < argc) {
		argsClone[i] = args[i];
		i += 1;
	}
	let frame = [null, pc, argsClone, argc, 0, 0, {}, null, false, null, false];
	let task = [ec[16], ec, frame, false, 0, PST$createNewArray(1), null];
	ec[16] += 1;
	ec[18][task[0]] = task;
	return task;
};

let createStringFromUnicodeArray = function(g, buffer, copyBuffer) {
	let uchars = buffer;
	let sz = buffer.length;
	if (sz < 2) {
		if (sz == 0) {
			return g[5];
		}
		let singleChar = String.fromCharCode(buffer[0]);
		return buildString(g, singleChar, true);
	}
	if (copyBuffer) {
		uchars = PST$createNewArray(sz);
		let i = 0;
		while (i < sz) {
			uchars[i] = buffer[i];
			i += 1;
		}
	}
	let sb = [];
	let j = 0;
	while (j < sz) {
		sb.push(String.fromCharCode(uchars[j]));
		j += 1;
	}
	let finalString = sb.join("");
	if (g[6][finalString] !== undefined) {
		return g[6][finalString];
	}
	return [5, [sz, false, uchars, finalString, null, null]];
};

let DictImpl_ensureCapacity = function(dict) {
	let size = dict[2];
	if (size < dict[3]) {
		return;
	}
	let newCapacity = dict[3] * 2;
	if (newCapacity < 4) {
		newCapacity = 4;
	}
	let newKeys = PST$createNewArray(newCapacity);
	let newValues = PST$createNewArray(newCapacity);
	let oldKeys = dict[4];
	let oldValues = dict[5];
	let i = 0;
	while (i < size) {
		newKeys[i] = oldKeys[i];
		newValues[i] = oldValues[i];
		i += 1;
	}
	dict[4] = newKeys;
	dict[5] = newValues;
	dict[3] = newCapacity;
};

let exceptionCatcherRouteException = function(exceptionInstance, args, outBuffer) {
	let inst = exceptionInstance[1];
	let exClassId = inst[1][0];
	let groupStartIndex = 1;
	while (groupStartIndex < args.length) {
		let groupJumpPc = args[groupStartIndex];
		let totalExceptionsChecked = 0;
		let i = groupStartIndex + 1;
		while (i < args.length) {
			groupStartIndex = i + 1;
			if (args[i] == 0) {
				i += args.length;
			} else {
				totalExceptionsChecked += 1;
				if (args[i] == exClassId) {
					outBuffer[0] = groupJumpPc;
					return false;
				}
			}
			i += 1;
		}
		if (totalExceptionsChecked == 0) {
			outBuffer[0] = groupJumpPc;
			return false;
		}
	}
	outBuffer[0] = args[0];
	return true;
};

let ExRes_Done = function(task) {
	return new_ExecutionResult(1, task);
};

let ExRes_HardCrash = function(task, message) {
	let isDebug = 1;
	let res = new_ExecutionResult(2, task);
	res[3] = message;
	if (isDebug == 1) {
		let failArgs = PST$createNewArray(1);
		failArgs[0] = message;
		let ignore = (PST$extCallbacks["hardCrash"] || ((o) => null))(failArgs);
	}
	return res;
};

let ExRes_Suspend = function(task, useSleep, sleepMillis) {
	let res = new_ExecutionResult(3, task);
	if (useSleep) {
		res[0] = 4;
		res[2] = sleepMillis;
	} else {
		res[2] = -1;
	}
	return res;
};

let finalizeExecutionContext = function(ec) {
	let byteCode = ec[9];
	let length = ec[9].length;
	let row = null;
	let i = 0;
	while (i < length) {
		row = byteCode[i];
		if (row[4] != 0) {
			row[5] = ec[12][row[4]];
		}
		if (row[6] != 0) {
			row[7] = ec[11][row[6]];
		}
		i += 1;
	}
};

let FunctionPointer_cloneWithNewType = function(old, newType) {
	return [newType, old[1], old[2], old[3], old[4], old[5]];
};

let generateNameLookup = function(ec) {
	let lookup = [0, null];
	let stringsToId = {};
	let i = 1;
	let sz = ec[12].length;
	while (i < sz) {
		stringsToId[ec[12][i]] = i;
		i += 1;
	}
	if (stringsToId["length"] !== undefined) {
		lookup[0] = stringsToId["length"];
	}
	let deadArray = PST$createNewArray(16);
	let fpMap = PST$createNewArray(sz);
	lookup[1] = fpMap;
	injectNameLookup(fpMap, 41, 8, tryGetNameId(stringsToId, "getIntBE"), 0, 2);
	injectNameLookup(fpMap, 42, 8, tryGetNameId(stringsToId, "getIntLE"), 0, 2);
	injectNameLookup(fpMap, 40, 8, tryGetNameId(stringsToId, "toBase64"), 0, 0);
	injectNameLookup(fpMap, 43, 8, tryGetNameId(stringsToId, "toString"), 0, 0);
	injectNameLookup(fpMap, 1, 10, tryGetNameId(stringsToId, "clear"), 0, 0);
	injectNameLookup(fpMap, 2, 10, tryGetNameId(stringsToId, "clone"), 0, 0);
	injectNameLookup(fpMap, 3, 10, tryGetNameId(stringsToId, "contains"), 1, 1);
	injectNameLookup(fpMap, 4, 10, tryGetNameId(stringsToId, "get"), 1, 2);
	injectNameLookup(fpMap, 5, 10, tryGetNameId(stringsToId, "keys"), 0, 0);
	injectNameLookup(fpMap, 6, 10, tryGetNameId(stringsToId, "mapKvp"), 1, 1);
	injectNameLookup(fpMap, 7, 10, tryGetNameId(stringsToId, "merge"), 1, 2);
	injectNameLookup(fpMap, 8, 10, tryGetNameId(stringsToId, "remove"), 1, 1);
	injectNameLookup(fpMap, 9, 10, tryGetNameId(stringsToId, "values"), 0, 0);
	injectNameLookup(fpMap, 10, 9, tryGetNameId(stringsToId, "add"), 1, 1);
	injectNameLookup(fpMap, 11, 9, tryGetNameId(stringsToId, "clear"), 0, 0);
	injectNameLookup(fpMap, 12, 9, tryGetNameId(stringsToId, "clone"), 0, 0);
	injectNameLookup(fpMap, 14, 9, tryGetNameId(stringsToId, "filter"), 1, 1);
	injectNameLookup(fpMap, 15, 9, tryGetNameId(stringsToId, "find"), 1, 3);
	injectNameLookup(fpMap, 16, 9, tryGetNameId(stringsToId, "findReverse"), 1, 3);
	injectNameLookup(fpMap, 17, 9, tryGetNameId(stringsToId, "join"), 0, 1);
	injectNameLookup(fpMap, 18, 9, tryGetNameId(stringsToId, "map"), 1, 1);
	injectNameLookup(fpMap, 19, 9, tryGetNameId(stringsToId, "pop"), 0, 0);
	injectNameLookup(fpMap, 21, 9, tryGetNameId(stringsToId, "reduce"), 1, 2);
	injectNameLookup(fpMap, 22, 9, tryGetNameId(stringsToId, "remove"), 1, 1);
	injectNameLookup(fpMap, 23, 9, tryGetNameId(stringsToId, "reverse"), 0, 0);
	injectNameLookup(fpMap, 20, 9, tryGetNameId(stringsToId, "sort"), 0, 1);
	injectNameLookup(fpMap, 39, 9, tryGetNameId(stringsToId, "sortByKey"), 1, 1);
	injectNameLookup(fpMap, 24, 9, tryGetNameId(stringsToId, "toBytes"), 0, 0);
	injectNameLookup(fpMap, 26, 5, tryGetNameId(stringsToId, "endsWith"), 1, 1);
	injectNameLookup(fpMap, 27, 5, tryGetNameId(stringsToId, "find"), 1, 3);
	injectNameLookup(fpMap, 28, 5, tryGetNameId(stringsToId, "findReverse"), 1, 3);
	injectNameLookup(fpMap, 29, 5, tryGetNameId(stringsToId, "getCodePoint"), 1, 1);
	injectNameLookup(fpMap, 30, 5, tryGetNameId(stringsToId, "lower"), 0, 0);
	injectNameLookup(fpMap, 31, 5, tryGetNameId(stringsToId, "replace"), 2, 3);
	injectNameLookup(fpMap, 32, 5, tryGetNameId(stringsToId, "split"), 1, 1);
	injectNameLookup(fpMap, 33, 5, tryGetNameId(stringsToId, "startsWith"), 1, 1);
	injectNameLookup(fpMap, 34, 5, tryGetNameId(stringsToId, "toBytes"), 0, 1);
	injectNameLookup(fpMap, 35, 5, tryGetNameId(stringsToId, "toChars"), 0, 0);
	injectNameLookup(fpMap, 36, 5, tryGetNameId(stringsToId, "toUnicodePoints"), 0, 0);
	injectNameLookup(fpMap, 37, 5, tryGetNameId(stringsToId, "trim"), 0, 0);
	injectNameLookup(fpMap, 38, 5, tryGetNameId(stringsToId, "upper"), 0, 0);
	i = 0;
	while (i < sz) {
		if (fpMap[i] == null) {
			fpMap[i] = deadArray;
		}
		i += 1;
	}
	ec[15] = lookup;
};

let generateStackTrace = function(task) {
	let frame = task[2];
	let ec = task[1];
	let trace = [];
	while (frame != null) {
		let invokeToken = ec[9][frame[1]][7];
		if (invokeToken != null) {
			let info = [invokeToken[0], " Line ", invokeToken[1] + '', " Col ", invokeToken[2] + ''].join('');
			trace.push(info);
		} else {
			trace.push("PC:" + (frame[1] + ''));
		}
		frame = frame[0];
	}
	let sz = trace.length;
	let traceArr = PST$createNewArray(sz);
	let i = 0;
	while (i < sz) {
		traceArr[i] = buildString(ec[1], trace[i], false);
		i += 1;
	}
	return buildList(ec, traceArr, false, traceArr.length);
};

let generateTryDescriptors = function(ec) {
	let bc = ec[9];
	let sz = bc.length;
	ec[10] = PST$createNewArray(sz);
	let i = 0;
	while (i < sz) {
		ec[10][i] = null;
		let row = bc[i];
		if (row[0] == 62) {
			let pcTry = i + row[1][0];
			let pcRouter = pcTry + row[1][1];
			let pcFinally = pcRouter + row[1][2];
			let pcEnd = pcFinally + row[1][3];
			let td = [pcTry, pcRouter, pcFinally, pcEnd];
			pcEnd -= 1;
			let j = pcTry;
			while (j < pcEnd) {
				ec[10][j] = td;
				j += 1;
			}
		}
		i += 1;
	}
};

let getExceptionMessage = function(exceptionInstance, includeStackTrace) {
	let inst = exceptionInstance[1];
	let msgField = inst[2][inst[1][8]["message"]];
	let lines = [];
	lines.push(valueToHumanString(msgField));
	if (includeStackTrace) {
		let stackTrace = inst[2][inst[1][8]["trace"]];
		if (stackTrace[0] == 9) {
			let trace = stackTrace[1];
			lines.push("Stack trace:");
			let i = 0;
			while (i < trace[1]) {
				lines.push("  at " + valueToHumanString(trace[3][i]));
				i += 1;
			}
		} else {
			lines.push("(no stack trace available)");
		}
	}
	return lines.join("\n");
};

let getGlobalsFromTask = function(taskObj) {
	return (taskObj)[1][1];
};

let increaseValueStackCapacity = function(task) {
	task[5] = valueArrayIncreaseCapacity(task[5]);
	return task[5];
};

let injectNameLookup = function(lookup, primitiveMethodId, typeId, nameId, argcMin, argcMax) {
	if (nameId == -1) {
		return;
	}
	let fp = [4, argcMin, argcMax, primitiveMethodId, null, null];
	if (lookup[nameId] == null) {
		lookup[nameId] = PST$createNewArray(16);
	}
	lookup[nameId][typeId] = fp;
};

let isValueEqual = function(a, b) {
	if (a[0] != b[0]) {
		if (a[0] == 3) {
			if (b[0] == 4) {
				return 0.0 + a[1] == b[1];
			}
		} else if (a[0] == 4) {
			if (b[0] == 3) {
				return a[1] == 0.0 + b[1];
			}
		}
		return false;
	}
	switch (a[0]) {
		case 1:
			return true;
		case 2:
			return a[1] == b[1];
		case 3:
			return a[1] == b[1];
		case 4:
			return a[1] == b[1];
		case 5:
			let s1 = a[1];
			let s2 = b[1];
			if (s1 == s2) {
				return true;
			}
			let sLen = s1[0];
			if (sLen != s2[0]) {
				return false;
			}
			if (s1[1]) {
				stringFlatten(s1);
			}
			if (s2[1]) {
				stringFlatten(s2);
			}
			if (sLen == 0) {
				return true;
			}
			if (s1[2][0] != s2[2][0]) {
				return false;
			}
			sLen -= 1;
			if (s1[2][sLen] != s2[2][sLen]) {
				return false;
			}
			let i = 1;
			while (i < sLen) {
				if (s1[2][i] != s2[2][i]) {
					return false;
				}
				i += 1;
			}
			return true;
		case 9:
			return (a[1])[0] == (b[1])[0];
		case 10:
			return (a[1])[0] == (b[1])[0];
		case 12:
			return (a[1])[0] == (b[1])[0];
		default:
			return a[1] == b[1];
	}
};

let List_add = function(o, v) {
	if (o[2] == o[1]) {
		List_expandCapacity(o);
	}
	o[3][o[1]] = v;
	o[1] += 1;
};

let List_expandCapacity = function(o) {
	let oldCapacity = o[2];
	let newCapacity = oldCapacity * 2;
	if (newCapacity < 4) {
		newCapacity = 4;
	}
	let newItems = PST$createNewArray(newCapacity);
	let oldItems = o[3];
	let i = 0;
	while (i < oldCapacity) {
		newItems[i] = oldItems[i];
		i += 1;
	}
	o[2] = newCapacity;
	o[3] = newItems;
};

let List_get = function(o, index) {
	if (index < 0) {
		index += o[1];
	}
	if (index < 0 || index >= o[1]) {
		return null;
	}
	return o[3][index];
};

let List_join = function(g, v, sep) {
	let o = v[1];
	if (o[1] == 0) {
		return g[5];
	}
	let output = [];
	output.push(valueToHumanString(o[3][0]));
	let sz = o[1];
	let i = 1;
	while (i < sz) {
		output.push(valueToHumanString(o[3][i]));
		i += 1;
	}
	let val = output.join(sep);
	return buildString(g, val, false);
};

let List_pop = function(o, v) {
	if (o[1] == 0) {
		return null;
	}
	o[1] -= 1;
	return o[3][o[1]];
};

let List_removeAt = function(o, index) {
	if (index < 0) {
		index += o[1];
	}
	if (index < 0 || index >= o[1]) {
		return false;
	}
	o[1] -= 1;
	while (index < o[1]) {
		o[3][index] = o[3][index + 1];
		index += 1;
	}
	return true;
};

let List_set = function(o, index, v) {
	if (index < 0) {
		index += o[1];
	}
	if (index < 0 || index >= o[1]) {
		return false;
	}
	o[3][index] = v;
	return true;
};

let new_ByteCodeRow = function(op, args, stringId, tokenId) {
	let arg1 = 0;
	let arg2 = 0;
	if (args.length > 0) {
		arg1 = args[0];
	}
	if (args.length > 1) {
		arg2 = args[1];
	}
	return [op, args, arg1, arg2, stringId, null, tokenId, null, null, false, false];
};

let new_ExecutionContext = function(rawBytes, extensions, appCtx) {
	let ec = [null, new_GlobalValues(), extensions, {}, null, null, null, null, null, null, null, null, null, null, null, null, 1, 1, {}, appCtx];
	let err = ParseRawData(rawBytes, ec);
	if (err == null) {
		ec[0] = "CORRUPT_EXECUTABLE";
	} else if (err != "OK") {
		ec[0] = err;
	} else {
		let i = 0;
		ec[5] = PST$createNewArray(ec[4].length);
		i = 1;
		while (i < ec[4].length) {
			ec[5][i] = buildFunctionFromInfo(ec[4][i]);
			i += 1;
		}
		ec[7] = PST$createNewArray(ec[6].length);
		i = 1;
		while (i < ec[6].length) {
			ec[7][i] = [13, ec[6][i]];
			i += 1;
		}
		ec[13] = PST$createNewArray(ec[9].length);
		ec[14] = PST$createNewArray(ec[9].length);
	}
	generateNameLookup(ec);
	return ec;
};

let new_ExecutionResult = function(type, task) {
	return [type, task, 0, null, null];
};

let new_GlobalValues = function() {
	let g = [[1, null], [2, true], [2, false], null, null, null, {}, null, null, PST$createNewArray(5)];
	g[3] = PST$createNewArray(1200);
	g[4] = PST$createNewArray(1200);
	let i = 0;
	while (i < 1200) {
		g[3][i] = [3, i];
		g[4][i] = [3, -i];
		i += 1;
	}
	g[7] = g[3][0];
	g[8] = g[3][1];
	g[4][0] = g[7];
	g[5] = buildString(g, "", true);
	let j = 0;
	while (j < 5) {
		g[9][j] = [4, j * 0.25];
		j += 1;
	}
	return g;
};

let ParseRaw_entitiesSection_classMemberResolver = function(classes, globalValues) {
	let j = 0;
	let id = 1;
	while (id < classes.length) {
		let cd = classes[id];
		cd[2] = classes[cd[1]];
		let newFieldCount = cd[11].length;
		let parentFieldCount = 0;
		if (cd[2] != null) {
			parentFieldCount = cd[2][9].length;
		}
		let fieldCount = newFieldCount + parentFieldCount;
		let nameByOffset = PST$createNewArray(fieldCount);
		let flatOffset = 0;
		while (flatOffset < parentFieldCount) {
			nameByOffset[flatOffset] = cd[2][9][flatOffset];
			flatOffset += 1;
		}
		let localOffset = 0;
		while (flatOffset < fieldCount) {
			nameByOffset[flatOffset] = cd[11][localOffset];
			flatOffset += 1;
			localOffset += 1;
		}
		cd[9] = nameByOffset;
		cd[8] = {};
		cd[10] = PST$createNewArray(fieldCount);
		let i = 0;
		while (i < fieldCount) {
			let memberName = nameByOffset[i];
			cd[8][memberName] = i;
			localOffset = i - parentFieldCount;
			if (localOffset < 0) {
				cd[10][i] = cd[2][10][i];
				if (cd[2][7][memberName] !== undefined && !(cd[7][memberName] !== undefined)) {
					cd[7][memberName] = cd[2][7][memberName];
				}
			} else {
				let memberInfoFlag = cd[12][localOffset];
				let isMethod = (memberInfoFlag & 1) != 0;
				if (isMethod) {
					cd[10][i] = null;
				} else {
					cd[10][i] = globalValues[0];
				}
			}
			i += 1;
		}
		id += 1;
	}
	return true;
};

let ParseRaw_entitiesSection_parseClasses = function(rdp, classCount, functions, globalValues) {
	let id = 0;
	let i = 0;
	let j = 0;
	let classes = [];
	classes.push(null);
	let cd = null;
	id = 1;
	while (id <= classCount) {
		let className = ParseRaw_popLenString(rdp);
		if (className == null) {
			return null;
		}
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let parentId = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let ctorId = rdp[3];
		if (ctorId >= functions.length || ctorId < 1) {
			return null;
		}
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let cctorId = rdp[3];
		if (cctorId >= functions.length || cctorId < 0) {
			return null;
		}
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let fieldCount = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let methodCount = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let staticFieldCount = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let staticMethodCount = rdp[3];
		let newMembersByOffset = [];
		let newMemberInfo = [];
		j = 0;
		while (j < fieldCount) {
			let memberName = ParseRaw_popLenString(rdp);
			if (memberName == null) {
				return null;
			}
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			newMemberInfo.push(rdp[3]);
			newMembersByOffset.push(memberName);
			j += 1;
		}
		cd = createClassInfo(id, parentId, className, functions[ctorId], newMembersByOffset, newMemberInfo, cctorId);
		functions[ctorId][3] = cd;
		j = 0;
		while (j < methodCount) {
			let methodName = ParseRaw_popLenString(rdp);
			if (methodName == null) {
				return null;
			}
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			let methodFuncId = rdp[3];
			cd[7][methodName] = functions[methodFuncId];
			j += 1;
		}
		j = 0;
		while (j < staticFieldCount) {
			let stFieldName = ParseRaw_popLenString(rdp);
			cd[14][stFieldName] = globalValues[0];
			cd[15][stFieldName] = true;
			j += 1;
		}
		j = 0;
		while (j < staticMethodCount) {
			let stMethodName = ParseRaw_popLenString(rdp);
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			let staticMethodFuncId = rdp[3];
			let staticMethodInfo = functions[staticMethodFuncId];
			cd[7][stMethodName] = staticMethodInfo;
			cd[14][stMethodName] = [11, [3, staticMethodInfo[0], staticMethodInfo[1], staticMethodInfo[2], staticMethodInfo, cd[13]]];
			cd[15][stMethodName] = false;
			j += 1;
		}
		classes.push(cd);
		id += 1;
	}
	return classes;
};

let ParseRaw_entitiesSection_parseEnums = function(rdp, enumCount) {
	let enums = [];
	enums.push(null);
	let i = 0;
	while (i < enumCount) {
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let memberCount = rdp[3];
		let id = i + 1;
		let enumValues = PST$createNewArray(memberCount);
		let enumNames = PST$createNewArray(memberCount);
		let j = 0;
		while (j < memberCount) {
			let enumName = ParseRaw_popLenString(rdp);
			if (enumName == null || !ParseRaw_popInt(rdp)) {
				return null;
			}
			enumNames[j] = enumName;
			enumValues[j] = rdp[3];
			j++;
		}
		enums.push([id, enumNames, enumValues]);
		i += 1;
	}
	return enums;
};

let ParseRaw_entitiesSection_parseFunctions = function(rdp, fnCount, byteCodeOut) {
	let functions = [];
	functions.push(null);
	let i = 0;
	while (i < fnCount) {
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let argcMin = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let argcMax = rdp[3];
		if (argcMax < argcMin) {
			return null;
		}
		let fnName = ParseRaw_popLenString(rdp);
		if (fnName == null) {
			return null;
		}
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let codeLen = rdp[3];
		let byteCode = ParseRaw_popByteCodeRows(rdp, codeLen);
		if (byteCode == null) {
			return null;
		}
		let pc = byteCodeOut.length;
		let fn = [argcMin, argcMax, pc, null, fnName];
		let j = 0;
		while (j < codeLen) {
			byteCodeOut.push(byteCode[j]);
			j += 1;
		}
		functions.push(fn);
		i += 1;
	}
	return functions;
};

let ParseRaw_parseEntityData = function(rdp, byteCodeOut, globalValues) {
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let fnCount = rdp[3];
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let enumCount = rdp[3];
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let classCount = rdp[3];
	let functions = ParseRaw_entitiesSection_parseFunctions(rdp, fnCount, byteCodeOut);
	if (functions == null) {
		return null;
	}
	let enums = ParseRaw_entitiesSection_parseEnums(rdp, enumCount);
	if (enums == null) {
		return null;
	}
	let classes = ParseRaw_entitiesSection_parseClasses(rdp, classCount, functions, globalValues);
	if (classes == null) {
		return null;
	}
	let ok = ParseRaw_entitiesSection_classMemberResolver(classes, globalValues);
	if (!ok) {
		return null;
	}
	return [functions, enums, classes];
};

let ParseRaw_parseMetadata = function(rdp) {
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let mainFn = rdp[3];
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let builtinCount = rdp[3];
	return [mainFn, builtinCount];
};

let ParseRaw_parseStringData = function(rdp) {
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let count = rdp[3];
	let output = PST$createNewArray(count + 1);
	output[0] = null;
	let i = 0;
	while (i < count) {
		let s = ParseRaw_popLenString(rdp);
		if (s == null) {
			return null;
		}
		output[i + 1] = s;
		i += 1;
	}
	return output;
};

let ParseRaw_parseTokenData = function(rdp) {
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let tokenCount = rdp[3];
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	let fileCount = rdp[3];
	let fileNames = [];
	while (fileCount > 0) {
		fileCount -= 1;
		let fileName = ParseRaw_popLenString(rdp);
		if (fileName == null) {
			return null;
		}
		fileNames.push(fileName);
	}
	let output = [];
	output.push(null);
	while (tokenCount > 0) {
		tokenCount -= 1;
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let fileId = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let line = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let col = rdp[3];
		if (fileId < 0 || fileId >= fileNames.length) {
			return null;
		}
		output.push([fileNames[fileId], line, col]);
	}
	return [...(output)];
};

let ParseRaw_popByteCodeRows = function(rdp, rowCount) {
	let rows = PST$createNewArray(rowCount);
	let i = 0;
	while (i < rowCount) {
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let op = rdp[3];
		if (!ParseRaw_popInt(rdp)) {
			return null;
		}
		let flags = rdp[3];
		let hasString = (flags & 1) != 0;
		let hasToken = (flags & 2) != 0;
		let argc = flags >> 2;
		let stringId = 0;
		if (hasString) {
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			stringId = rdp[3];
		}
		let tokenId = 0;
		if (hasToken) {
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			tokenId = rdp[3];
		}
		let args = PST$createNewArray(argc);
		let j = 0;
		while (j < argc) {
			if (!ParseRaw_popInt(rdp)) {
				return null;
			}
			args[j] = rdp[3];
			j += 1;
		}
		rows[i] = new_ByteCodeRow(op, args, stringId, tokenId);
		i += 1;
	}
	return rows;
};

let ParseRaw_popBytes = function(rdp, byteCount) {
	if (rdp[1] + byteCount > rdp[2]) {
		return null;
	}
	let output = PST$createNewArray(byteCount);
	let i = 0;
	while (i < byteCount) {
		output[i] = rdp[0][i + rdp[1]];
		i += 1;
	}
	rdp[1] += byteCount;
	return output;
};

let ParseRaw_popFixedLenString = function(rdp, expectedSize) {
	if (rdp[1] + expectedSize > rdp[2]) {
		return null;
	}
	let strBytes = PST$createNewArray(expectedSize);
	let i = 0;
	while (i < expectedSize) {
		strBytes[i] = rdp[0][rdp[1] + i] & 255;
		i += 1;
	}
	rdp[1] += expectedSize;
	let value = new TextDecoder().decode(new Uint8Array(strBytes));
	return value;
};

let ParseRaw_popInt = function(rdp) {
	if (rdp[1] >= rdp[2]) {
		return false;
	}
	let b = rdp[0][rdp[1]];
	if (b < 128) {
		rdp[3] = b;
		rdp[1] += 1;
		return true;
	}
	if (b == 192) {
		rdp[3] = -2147483647;
		rdp[1] += 1;
		return true;
	}
	if (b == 224) {
		rdp[1] += 1;
		return false;
	}
	let sign = 1;
	if ((b & 16) != 0) {
		sign = -1;
	}
	let byteCount = b & 15;
	let output = 0;
	rdp[1] += 1;
	let i = 0;
	while (i < byteCount) {
		output = output << 8;
		output += rdp[0][rdp[1]];
		rdp[1] += 1;
		i += 1;
	}
	rdp[3] = output * sign;
	return true;
};

let ParseRaw_popLenString = function(rdp) {
	if (!ParseRaw_popInt(rdp)) {
		return null;
	}
	return ParseRaw_popFixedLenString(rdp, rdp[3]);
};

let ParseRaw_popSingleByte = function(rdp, fallback) {
	if (rdp[1] >= rdp[2]) {
		return fallback;
	}
	let val = rdp[0][rdp[1]];
	rdp[1] += 1;
	return val;
};

let ParseRawData = function(rawBytes, ec) {
	let rdp = [rawBytes, 0, rawBytes.length, 0];
	if (ParseRaw_popFixedLenString(rdp, 4) != "PXCS") {
		return null;
	}
	if (ParseRaw_popSingleByte(rdp, -1) != 0) {
		return null;
	}
	let versionData = ParseRaw_popBytes(rdp, 3);
	if (versionData == null) {
		return null;
	}
	let majorVersion = versionData[0];
	let minorVersion = versionData[1];
	let patchVersion = versionData[2];
	let flavor = ParseRaw_popLenString(rdp);
	let flavorVersion = ParseRaw_popLenString(rdp);
	if (flavor == null || flavorVersion == null) {
		return null;
	}
	let mtd = null;
	let stringById = null;
	let tokensById = null;
	let ent = null;
	let byteCodeAcc = [];
	while (rdp[1] < rdp[2]) {
		let chunkType = ParseRaw_popFixedLenString(rdp, 3);
		if (chunkType == null) {
			return null;
		}
		if (chunkType == "MTD") {
			if (mtd != null) {
				return null;
			}
			mtd = ParseRaw_parseMetadata(rdp);
			if (mtd == null) {
				return null;
			}
		} else if (chunkType == "TOK") {
			if (tokensById != null) {
				return null;
			}
			tokensById = ParseRaw_parseTokenData(rdp);
			if (tokensById == null) {
				return null;
			}
		} else if (chunkType == "STR") {
			if (stringById != null) {
				return null;
			}
			stringById = ParseRaw_parseStringData(rdp);
			if (stringById == null) {
				return null;
			}
		} else if (chunkType == "ENT") {
			if (ent != null) {
				return null;
			}
			ent = ParseRaw_parseEntityData(rdp, byteCodeAcc, ec[1]);
			if (ent == null) {
				return null;
			}
		} else {
			return null;
		}
	}
	if (mtd == null || stringById == null || ent == null) {
		return null;
	}
	ec[4] = [...(ent[0])];
	ec[8] = [...(ent[1])];
	ec[6] = [...(ent[2])];
	ec[9] = [...(byteCodeAcc)];
	ec[11] = tokensById;
	ec[12] = stringById;
	ec[3]["main"] = mtd[0];
	let i = 1;
	while (i <= mtd[1]) {
		let fnName = ec[4][i][4];
		if (fnName == "map" || fnName == "filter" || fnName == "reduce" || fnName == "thrw" || fnName == "sort" || fnName == "sortK") {
			ec[3][fnName] = i;
		}
		i += 1;
	}
	generateTryDescriptors(ec);
	return "OK";
};

let PUBLIC_getApplicationContextFromTask = function(taskObj) {
	let task = taskObj;
	return task[1][19];
};

let PUBLIC_getExecutionContextError = function(ecObj) {
	let ec = ecObj;
	return ec[0];
};

let PUBLIC_getTaskResultError = function(resObj, includeStackTrace) {
	let result = resObj;
	let o = [];
	if (result[0] != 2) {
		return null;
	}
	o.push(result[3]);
	let tr = result[4];
	if (tr != null) {
		let i = 0;
		while (i < tr.length) {
			o.push(tr[i]);
			i += 1;
		}
	}
	return [...(o)];
};

let PUBLIC_getTaskResultSleepAmount = function(resObj) {
	let result = resObj;
	return result[2];
};

let PUBLIC_getTaskResultStatus = function(resObj) {
	let result = resObj;
	return result[0];
};

let PUBLIC_initializeExecutionContext = function(rawBytes, extensions, appCtx) {
	return new_ExecutionContext(rawBytes, extensions, appCtx);
};

let PUBLIC_listValueAdd = function(listObj, wrappedValue) {
	let list = (listObj)[1];
	List_add(list, wrappedValue);
};

let PUBLIC_requestTaskSuspension = function(taskObj, withSleep, sleepMillis) {
	let task = taskObj;
	task[3] = true;
	if (sleepMillis < 0) {
		sleepMillis = 0;
	}
	if (withSleep) {
		task[4] = sleepMillis;
	} else {
		task[4] = -1;
	}
};

let PUBLIC_startMainTask = function(ecObj, args) {
	let mainTask = createMainTask(ecObj, args);
	return RunInterpreter(mainTask);
};

let PUBLIC_unwrapInteger = function(val) {
	return (val)[1];
};

let PUBLIC_unwrapNativeHandle = function(val) {
	return (val)[1];
};

let PUBLIC_valueToString = function(valueObj) {
	return valueToHumanString(valueObj);
};

let PUBLIC_wrapBoolean = function(taskObj, val) {
	let g = getGlobalsFromTask(taskObj);
	if (val) {
		return g[1];
	}
	return g[2];
};

let PUBLIC_wrapInteger = function(taskObj, val) {
	let g = getGlobalsFromTask(taskObj);
	return buildInteger(g, val);
};

let PUBLIC_wrapNativeHandle = function(val) {
	return [14, val];
};

let PUBLIC_wrapString = function(taskObj, val, isCommon) {
	let g = getGlobalsFromTask(taskObj);
	return buildString(g, val, isCommon);
};

let RunInterpreter = function(task) {
	let reinvoke = true;
	let result = null;
	while (reinvoke) {
		result = RunInterpreterImpl(task);
		reinvoke = result[0] == 5;
	}
	return result;
};

let RunInterpreterImpl = function(task) {
	let ec = task[1];
	let frame = task[2];
	let nextFrame = null;
	let byteCode = ec[9];
	let row = null;
	let globalValues = ec[1];
	let td = null;
	let nameLookup = ec[15];
	let LENGTH_ID = nameLookup[0];
	let primitiveMethodLookup = nameLookup[1];
	let pc = frame[1];
	let valueStack = task[5];
	let valueStackSize = frame[4];
	let valueStackCapacity = valueStack.length;
	let locals = frame[6];
	let interrupt = false;
	let doInvoke = false;
	let overrideReturnValueWithContext = false;
	let errorId = 0;
	let errorMsg = null;
	let i = 0;
	let j = 0;
	let argc = 0;
	let int1 = 0;
	let int2 = 0;
	let int3 = 0;
	let sz = 0;
	let leftType = 0;
	let rightType = 0;
	let bool1 = true;
	let bool2 = true;
	let bool3 = true;
	let str1 = null;
	let str2 = null;
	let name = null;
	let float1 = 0.0;
	let float2 = 0.0;
	let object1 = null;
	let value = null;
	let value1 = null;
	let value2 = null;
	let output = null;
	let left = null;
	let right = null;
	let fp = null;
	let fn = null;
	let stringImpl1 = null;
	let stringImpl2 = null;
	let listImpl1 = null;
	let listImpl2 = null;
	let dictImpl1 = null;
	let instance1 = null;
	let classDef = null;
	let args = null;
	let valueArr = null;
	let intArray1 = null;
	let objArr = null;
	let keys = null;
	let values = null;
	let extensionFunc = null;
	let opMap = null;
	let str2FuncDef = null;
	let str2Val = null;
	let switchIntLookup = null;
	let switchStrLookup = null;
	let intList = null;
	let valueList = null;
	let VALUE_TRUE = globalValues[1];
	let VALUE_FALSE = globalValues[2];
	let VALUE_NULL = globalValues[0];
	let value16 = PST$createNewArray(16);
	let intBuffer16 = PST$createNewArray(16);
	while (true) {
		row = byteCode[pc];
		switch (row[0]) {
			case 1:
				// OP_ASSIGN_FIELD;
				valueStackSize -= 2;
				if (row[2] == 0) {
					right = valueStack[valueStackSize];
					left = valueStack[valueStackSize + 1];
				} else {
					left = valueStack[valueStackSize];
					right = valueStack[valueStackSize + 1];
				}
				switch (left[0]) {
					case 12:
						instance1 = left[1];
						if (!(instance1[1][8][row[5]] !== undefined)) {
							errorId = 3;
							errorMsg = "Instance does not have that field";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						} else {
							int1 = instance1[1][8][row[5]];
							instance1[2][int1] = right;
						}
						break;
					case 13:
						classDef = left[1];
						str1 = row[5];
						if (classDef[15][str1]) {
							classDef[14][str1] = right;
						} else {
							errorId = 11;
							errorMsg = "Static methods cannot be overwritten.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						break;
					default:
						errorId = 3;
						errorMsg = "Cannot assign fields on this type.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 2:
				// OP_ASSIGN_INDEX;
				valueStackSize -= 3;
				if (row[2] == 0) {
					value = valueStack[valueStackSize];
					left = valueStack[valueStackSize + 1];
					right = valueStack[valueStackSize + 2];
				} else {
					left = valueStack[valueStackSize];
					right = valueStack[valueStackSize + 1];
					value = valueStack[valueStackSize + 2];
				}
				switch (left[0] * 16 + right[0]) {
					case 147:
						i = right[1];
						listImpl1 = left[1];
						if (i < 0) {
							i += listImpl1[1];
							if (i < 0) {
								bool1 = true;
							} else {
								bool1 = false;
							}
						} else if (i >= listImpl1[1]) {
							bool1 = true;
						} else {
							bool1 = false;
						}
						if (bool1) {
							errorId = 8;
							errorMsg = "Array is out of bounds.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						} else {
							listImpl1[3][i] = value;
						}
						break;
					case 165:
						stringImpl1 = right[1];
						if (stringImpl1[1]) {
							stringFlatten(stringImpl1);
						}
						str1 = stringImpl1[3];
						dictImpl1 = left[1];
						if (dictImpl1[2] == dictImpl1[3]) {
							DictImpl_ensureCapacity(dictImpl1);
						}
						if (dictImpl1[1] == 1) {
							dictImpl1[1] = 5;
							dictImpl1[7] = {};
							dictImpl1[7][str1] = 0;
							dictImpl1[4][0] = right;
							dictImpl1[5][0] = value;
							dictImpl1[2] = 1;
						} else if (dictImpl1[1] == 5) {
							if (dictImpl1[7][str1] !== undefined) {
								dictImpl1[5][dictImpl1[7][str1]] = value;
							} else {
								i = dictImpl1[2];
								dictImpl1[2] = i + 1;
								dictImpl1[4][i] = right;
								dictImpl1[5][i] = value;
								dictImpl1[7][str1] = i;
							}
						} else {
							errorId = 4;
							errorMsg = "Cannot mix types of dictionary keys in the same dictionary.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						break;
					case 163:
						int1 = right[1];
						dictImpl1 = left[1];
						if (dictImpl1[2] == dictImpl1[3]) {
							DictImpl_ensureCapacity(dictImpl1);
						}
						if (dictImpl1[1] == 1) {
							dictImpl1[1] = 3;
							dictImpl1[6] = {};
							dictImpl1[6][int1] = 0;
							dictImpl1[4][0] = right;
							dictImpl1[5][0] = value;
							dictImpl1[2] = 1;
						} else if (dictImpl1[6][int1] !== undefined) {
							dictImpl1[5][dictImpl1[6][int1]] = value;
						} else {
							i = dictImpl1[2];
							dictImpl1[2] = i + 1;
							dictImpl1[4][i] = right;
							dictImpl1[5][i] = value;
							dictImpl1[6][int1] = i;
						}
						break;
					default:
						errorId = 4;
						errorMsg = "Cannot assign an index to this type.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 3:
				// OP_ASSIGN_VAR;
				valueStackSize -= 1;
				locals[row[5]] = valueStack[valueStackSize];
				break;
			case 4:
				// OP_BIN_OP;
				if (opMap == null) {
					opMap = {};
					opMap["+"] = 1;
					opMap["-"] = 2;
					opMap["*"] = 3;
					opMap["/"] = 4;
					opMap["%"] = 5;
					opMap["&&"] = 6;
					opMap["||"] = 7;
					opMap["=="] = 8;
					opMap["!="] = 9;
					opMap[">"] = 10;
					opMap["<"] = 11;
					opMap[">="] = 12;
					opMap["<="] = 13;
					opMap["&"] = 14;
					opMap["|"] = 15;
					opMap["^"] = 16;
					opMap["<<"] = 17;
					opMap[">>"] = 18;
					opMap[">>>"] = 19;
				}
				str1 = row[5];
				if (str1 == "+") {
					row[0] = 5;
				} else if (str1 == "==" || str1 == "!=") {
					row[9] = str1 == "!=";
					row[0] = 8;
				} else if (str1 == "<" || str1 == ">" || str1 == "<=" || str1 == ">=") {
					row[9] = str1 == "<" || str1 == "<=";
					row[10] = str1 == "<=" || str1 == ">=";
					row[2] = opMap[str1];
					row[0] = 7;
				} else if (str1 == "&" || str1 == "|" || str1 == "^" || str1 == "<<" || str1 == ">>" || str1 == ">>>") {
					row[2] = opMap[str1];
					row[0] = 6;
				} else {
					row[2] = opMap[str1];
					row[0] = 9;
				}
				pc -= 1;
				break;
			case 5:
				// OP_BIN_OP_ADD;
				valueStackSize -= 2;
				left = valueStack[valueStackSize];
				right = valueStack[valueStackSize + 1];
				switch ((left[0] << 5) | right[0]) {
					case 99:
						i = left[1] + right[1];
						if (i < 1200 && i >= 1200) {
							if (i < 0) {
								value = globalValues[4][-i];
							} else {
								value = globalValues[3][i];
							}
						} else {
							value = [3, i];
						}
						break;
					case 132:
						value = [4, left[1] + right[1]];
						break;
					case 131:
						value = [4, left[1] + right[1]];
						break;
					case 100:
						value = [4, left[1] + right[1]];
						break;
					default:
						if (left[0] == 5 || right[0] == 5) {
							stringImpl1 = convertToStringImpl(globalValues, left);
							stringImpl2 = convertToStringImpl(globalValues, right);
							stringImpl1 = [stringImpl1[0] + stringImpl2[0], true, null, null, stringImpl1, stringImpl2];
							value = [5, stringImpl1];
						} else {
							errorId = 4;
							errorMsg = "Unsupported addition operation between types.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						break;
				}
				valueStack[valueStackSize] = value;
				valueStackSize += 1;
				break;
			case 7:
				// OP_BIN_OP_COMPARE;
				valueStackSize -= 2;
				left = valueStack[valueStackSize];
				right = valueStack[valueStackSize + 1];
				switch (left[0] * 16 + right[0]) {
					case 51:
						int1 = left[1];
						int2 = right[1];
						bool1 = int1 == int2;
						bool2 = int1 < int2;
						break;
					case 52:
						int1 = left[1];
						float2 = right[1];
						bool1 = int1 == float2;
						bool2 = int1 < float2;
						break;
					case 67:
						float1 = left[1];
						int2 = right[1];
						bool1 = float1 == int2;
						bool2 = float1 < int2;
						break;
					case 68:
						float1 = left[1];
						float2 = right[1];
						bool1 = float1 == float2;
						bool2 = float1 < float2;
						break;
					default:
						errorId = 4;
						errorMsg = "Comparisons are only applicable for numeric types.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				value = VALUE_FALSE;
				switch (row[2]) {
					case 11:
						if (bool2) {
							value = VALUE_TRUE;
						}
						break;
					case 13:
						if (bool2 || bool1) {
							value = VALUE_TRUE;
						}
						break;
					case 10:
						if (!bool2 && !bool1) {
							value = VALUE_TRUE;
						}
						break;
					case 12:
						if (!bool2) {
							value = VALUE_TRUE;
						}
						break;
					default:
						break;
				}
				valueStack[valueStackSize] = value;
				valueStackSize += 1;
				break;
			case 8:
				// OP_BIN_OP_EQUAL;
				valueStackSize -= 2;
				left = valueStack[valueStackSize];
				right = valueStack[valueStackSize + 1];
				if (left[0] != right[0]) {
					if (left[0] == 3) {
						if (right[0] == 4) {
							int1 = left[1];
							bool1 = int1 == right[1];
						} else {
							bool1 = false;
						}
					} else if (left[0] == 4) {
						if (left[0] == 3) {
							int1 = right[1];
							bool1 = int1 == left[1];
						} else {
							bool1 = false;
						}
					} else {
						bool1 = false;
					}
				} else {
					switch (left[0]) {
						case 1:
							bool1 = true;
							break;
						case 2:
							bool1 = left[1] == right[1];
							break;
						case 3:
							bool1 = left[1] == right[1];
							break;
						case 4:
							bool1 = left[1] == right[1];
							break;
						case 5:
							if (left[1] == right[1]) {
								bool1 = true;
							} else {
								stringImpl1 = left[1];
								stringImpl2 = right[1];
								if (stringImpl1[0] != stringImpl2[0]) {
									bool1 = false;
								} else {
									if (stringImpl1[1]) {
										stringFlatten(stringImpl1);
									}
									if (stringImpl2[1]) {
										stringFlatten(stringImpl2);
									}
									bool1 = stringImpl1[3] === stringImpl2[3];
								}
							}
							break;
						default:
							bool1 = left[1] == right[1];
							break;
					}
				}
				if (row[9]) {
					bool1 = !bool1;
				}
				if (bool1) {
					valueStack[valueStackSize] = globalValues[1];
				} else {
					valueStack[valueStackSize] = globalValues[2];
				}
				valueStackSize += 1;
				break;
			case 9:
				// OP_BIN_OP_MATH;
				valueStackSize -= 2;
				left = valueStack[valueStackSize];
				right = valueStack[valueStackSize + 1];
				switch ((left[0] * 20 + row[2]) * 16 + right[0]) {
					case 995:
						int1 = left[1];
						int2 = right[1];
						value = buildInteger(globalValues, int1 - int2);
						break;
					case 1011:
						int1 = left[1];
						int2 = right[1];
						value = buildInteger(globalValues, int1 * int2);
						break;
					case 1331:
						value = [4, left[1] * right[1]];
						break;
					case 1012:
						value = [4, left[1] * right[1]];
						break;
					case 1332:
						value = [4, left[1] * right[1]];
						break;
					case 1027:
						int1 = left[1];
						int2 = right[1];
						if (int2 == 0) {
							errorId = 10;
							errorMsg = "Cannot divide by zero";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						value = buildInteger(globalValues, Math.floor(int1 / int2));
						break;
					case 1347:
						float1 = left[1];
						if (float1 == 0) {
							errorId = 10;
							errorMsg = "Cannot divide by zero";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						value = [4, float1 / right[1]];
						break;
					case 1028:
						int1 = left[1];
						if (int1 == 0) {
							errorId = 10;
							errorMsg = "Cannot divide by zero";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						value = [4, int1 / right[1]];
						break;
					case 1348:
						float1 = left[1];
						if (float1 == 0) {
							errorId = 10;
							errorMsg = "Cannot divide by zero";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						value = [4, float1 / right[1]];
						break;
					case 1043:
						int1 = left[1];
						int2 = right[1];
						if (int2 <= 0) {
							errorId = 10;
							errorMsg = "Modulo only applicable to positive divisors.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						int1 = int1 % int2;
						if (int1 < 0) {
							int1 += int2;
						}
						value = buildInteger(globalValues, int1);
						break;
					case 1364:
						if (left[0] == 3) {
							float1 = 0.0 + left[1];
						} else {
							float1 = left[1];
						}
						if (right[0] == 3) {
							float2 = 0.0 + right[1];
						} else {
							float2 = right[1];
						}
						if (float2 <= 0) {
							errorId = 10;
							errorMsg = "Modulo only applicable to positive divisors.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						float1 = float1 % float2;
						if (float1 < 0) {
							float1 += float2;
						}
						value = [4, float1];
						break;
					case 1363:
						if (left[0] == 3) {
							float1 = 0.0 + left[1];
						} else {
							float1 = left[1];
						}
						if (right[0] == 3) {
							float2 = 0.0 + right[1];
						} else {
							float2 = right[1];
						}
						if (float2 <= 0) {
							errorId = 10;
							errorMsg = "Modulo only applicable to positive divisors.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						float1 = float1 % float2;
						if (float1 < 0) {
							float1 += float2;
						}
						value = [4, float1];
						break;
					case 1044:
						if (left[0] == 3) {
							float1 = 0.0 + left[1];
						} else {
							float1 = left[1];
						}
						if (right[0] == 3) {
							float2 = 0.0 + right[1];
						} else {
							float2 = right[1];
						}
						if (float2 <= 0) {
							errorId = 10;
							errorMsg = "Modulo only applicable to positive divisors.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						float1 = float1 % float2;
						if (float1 < 0) {
							float1 += float2;
						}
						value = [4, float1];
						break;
					case 1316:
						value = [4, left[1] - right[1]];
						break;
					case 996:
						value = [4, left[1] - right[1]];
						break;
					case 1315:
						value = [4, left[1] - right[1]];
						break;
					case 1651:
						if (left[0] == 5) {
							value2 = left;
							stringImpl1 = left[1];
							sz = right[1];
						} else {
							value2 = right;
							stringImpl1 = right[1];
							sz = left[1];
						}
						if (sz == 0) {
							value = globalValues[5];
						} else if (sz == 1) {
							value = value2;
						} else {
							stringImpl2 = stringImpl1;
							i = 1;
							while (i < sz) {
								stringImpl2 = [stringImpl1[0] + stringImpl2[0], true, null, null, stringImpl1, stringImpl2];
								i += 1;
							}
							value = [5, stringImpl2];
						}
						break;
					case 1013:
						if (left[0] == 5) {
							value2 = left;
							stringImpl1 = left[1];
							sz = right[1];
						} else {
							value2 = right;
							stringImpl1 = right[1];
							sz = left[1];
						}
						if (sz == 0) {
							value = globalValues[5];
						} else if (sz == 1) {
							value = value2;
						} else {
							stringImpl2 = stringImpl1;
							i = 1;
							while (i < sz) {
								stringImpl2 = [stringImpl1[0] + stringImpl2[0], true, null, null, stringImpl1, stringImpl2];
								i += 1;
							}
							value = [5, stringImpl2];
						}
						break;
					default:
						errorId = 4;
						errorMsg = "Math operator not defined for these types";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				valueStack[valueStackSize] = value;
				valueStackSize += 1;
				break;
			case 6:
				// OP_BIN_OP_BIT_MATH;
				valueStackSize -= 2;
				left = valueStack[valueStackSize];
				right = valueStack[valueStackSize + 1];
				if (left[0] != 3 || right[0] != 3) {
					errorId = 9;
					errorMsg = "Expected integers for this operator.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				int1 = left[1];
				int2 = right[1];
				switch (row[2]) {
					case 14:
						int3 = int1 & int2;
						break;
					case 15:
						int3 = int1 | int2;
						break;
					case 16:
						int3 = int1 ^ int2;
						break;
					case 17:
						int3 = int1 << int2;
						break;
					case 18:
						int3 = int1 >> int2;
						break;
					default:
						return ExRes_HardCrash(task, "Op not implemented yet");
				}
				if (int1 == int3) {
					value = left;
				} else if (int2 == int3) {
					value = right;
				} else {
					value = buildInteger(globalValues, int3);
				}
				valueStack[valueStackSize] = value;
				valueStackSize += 1;
				break;
			case 10:
				// OP_BITWISE_NOT;
				i = valueStackSize - 1;
				value = valueStack[i];
				int1 = (-value[1]) - 1;
				valueStack[i] = buildInteger(globalValues, int1);
				break;
			case 11:
				// OP_BOOLEAN_NOT;
				i = valueStackSize - 1;
				value = valueStack[i];
				if (value[0] != 2) {
					errorId = 9;
					errorMsg = "Only a boolean can be used here.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				if (value[1]) {
					valueStack[i] = VALUE_FALSE;
				} else {
					valueStack[i] = VALUE_TRUE;
				}
				break;
			case 13:
				// OP_BUILD_DICT;
				sz = row[2];
				int1 = 1;
				if (sz == 0) {
					if (valueStackSize == valueStackCapacity) {
						valueStack = increaseValueStackCapacity(task);
						valueStackCapacity = valueStack.length;
					}
					int2 = 0;
					keys = null;
					values = null;
				} else {
					int2 = sz;
					keys = PST$createNewArray(sz);
					values = PST$createNewArray(sz);
					valueStackSize -= sz * 2;
					j = valueStackSize;
					i = 0;
					while (i < sz) {
						keys[i] = valueStack[j];
						values[i] = valueStack[j + 1];
						j += 2;
						i += 1;
					}
					int1 = keys[0][0];
				}
				dictImpl1 = [ec[17], int1, sz, int2, keys, values, null, null];
				ec[17] += 1;
				if (dictImpl1[1] == 3) {
					dictImpl1[6] = {};
					i = 0;
					while (i < sz) {
						dictImpl1[6][keys[i][1]] = i;
						i += 1;
					}
				} else if (dictImpl1[1] == 5) {
					dictImpl1[7] = {};
					i = 0;
					while (i < sz) {
						value = keys[i];
						stringImpl1 = value[1];
						if (stringImpl1[1]) {
							stringFlatten(stringImpl1);
						}
						dictImpl1[7][stringImpl1[3]] = i;
						i += 1;
					}
				}
				valueStack[valueStackSize] = [10, dictImpl1];
				valueStackSize += 1;
				keys = null;
				values = null;
				break;
			case 12:
				// OP_BUILD_LIST;
				sz = row[2];
				args = PST$createNewArray(sz);
				valueStackSize -= sz;
				i = 0;
				while (i < sz) {
					args[i] = valueStack[valueStackSize + i];
					i += 1;
				}
				if (sz == 0) {
					if (valueStackSize == valueStackCapacity) {
						valueStack = increaseValueStackCapacity(task);
						valueStackCapacity = valueStack.length;
					}
				}
				valueStack[valueStackSize] = [9, [ec[17], sz, sz, args]];
				valueStackSize += 1;
				ec[17] += 1;
				args = null;
				break;
			case 14:
				// OP_CTOR_REF;
				int1 = row[2];
				row[8] = ec[6][int1][4];
				row[0] = 44;
				pc -= 1;
				break;
			case 15:
				// OP_DOT_FIELD;
				i = valueStackSize - 1;
				value = valueStack[i];
				int3 = -1;
				j = row[4];
				name = row[5];
				switch (value[0]) {
					case 8:
						if (j == LENGTH_ID) {
							sz = (value[1]).length;
							if (sz < 1200) {
								output = globalValues[3][sz];
							} else {
								output = buildInteger(globalValues, sz);
							}
						} else {
							fp = primitiveMethodLookup[j][8];
							if (fp == null) {
								output = null;
							} else {
								output = [11, [4, fp[1], fp[2], fp[3], null, value]];
							}
						}
						break;
					case 5:
						if (j == LENGTH_ID) {
							sz = (value[1])[0];
							if (sz < 1200) {
								output = globalValues[3][sz];
							} else {
								output = buildInteger(globalValues, sz);
							}
						} else {
							fp = primitiveMethodLookup[j][5];
							if (fp == null) {
								output = null;
							} else {
								output = [11, [4, fp[1], fp[2], fp[3], null, value]];
							}
						}
						break;
					case 9:
						if (j == LENGTH_ID) {
							sz = (value[1])[1];
							if (sz < 1200) {
								output = globalValues[3][sz];
							} else {
								output = buildInteger(globalValues, sz);
							}
						} else {
							fp = primitiveMethodLookup[j][9];
							if (fp == null) {
								output = null;
							} else {
								output = [11, [4, fp[1], fp[2], fp[3], null, value]];
							}
						}
						break;
					case 10:
						fp = primitiveMethodLookup[j][10];
						if (fp == null) {
							output = null;
						} else {
							output = [11, [4, fp[1], fp[2], fp[3], null, value]];
						}
						break;
					case 12:
						instance1 = value[1];
						if (!(instance1[1][8][name] !== undefined)) {
							errorId = 3;
							errorMsg = "Instance does not contain that field.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						int1 = instance1[1][8][name];
						output = instance1[2][int1];
						if (output == null) {
							str2FuncDef = instance1[1][7];
							fn = str2FuncDef[name];
							fp = [2, fn[0], fn[1], fn[2], fn, value];
							output = [11, fp];
							instance1[2][int1] = output;
						}
						break;
					case 13:
						str2Val = (value[1])[14];
						if (!(str2Val[name] !== undefined)) {
							frame[1] = pc;
							frame[4] = valueStackSize;
							task[2] = frame;
							return ExRes_HardCrash(task, "TODO: all static fields need to be initialized. This should eventually not happen.");
						}
						output = str2Val[name];
						break;
					default:
						output = null;
						break;
				}
				if (output == null) {
					errorId = 3;
					errorMsg = "Field not found: ." + row[5];
					if (value[0] == 1) {
						errorId = 7;
						errorMsg = "Cannot access fields on null.";
						if (byteCode[pc - 1][0] == 21) {
							errorMsg = "The function returned null and the field could not be accessed.";
						}
					}
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				valueStack[i] = output;
				break;
			case 16:
				// OP_ENSURE_BOOL;
				if (valueStack[valueStackSize - 1][0] != 2) {
					errorId = 9;
					errorMsg = "Expected a boolean here.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 18:
				// OP_ENSURE_INT;
				if (valueStack[valueStackSize - 1][0] != 3) {
					errorId = 9;
					errorMsg = "Expected an integer here.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 17:
				// OP_ENSURE_INT_OR_STRING;
				i = valueStack[valueStackSize - 1][0];
				if (i != 3 && i != 5) {
					errorId = 9;
					errorMsg = "Expected an integer or string here.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 19:
				// OP_ENSURE_STRING;
				if (valueStack[valueStackSize - 1][0] != 5) {
					errorId = 9;
					errorMsg = "Expected a string here.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 20:
				// OP_EXT_INVOKE;
				name = row[5];
				argc = row[2];
				if (ec[2][name] !== undefined) {
					objArr = PST$createNewArray(argc);
					i = argc - 1;
					while (i >= 0) {
						valueStackSize -= 1;
						objArr[i] = valueStack[valueStackSize];
						i -= 1;
					}
					frame[1] = pc;
					frame[4] = valueStackSize;
					task[2] = frame;
					extensionFunc = ec[2][name];
					value = extensionFunc(task, objArr);
					objArr = null;
					if (task[3]) {
						frame[1] += 1;
						task[3] = false;
						return ExRes_Suspend(task, task[4] >= 0, task[4]);
					}
					if (value == null) {
						value = globalValues[0];
					}
					valueStack[valueStackSize] = value;
					valueStackSize += 1;
				} else {
					errorId = 5;
					errorMsg = ["There is no extension function named '", name, "'."].join('');
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 21:
				// OP_FUNCTION_INVOKE;
				argc = row[2];
				args = PST$createNewArray(argc);
				i = argc - 1;
				while (i >= 0) {
					valueStackSize -= 1;
					args[i] = valueStack[valueStackSize];
					i -= 1;
				}
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				if (value[0] != 11) {
					errorId = 5;
					errorMsg = "This is not a function";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				fp = value[1];
				if (argc < fp[1] || argc > fp[2]) {
					errorId = 5;
					errorMsg = "Incorrect number of arguments.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				doInvoke = false;
				switch (fp[0]) {
					case 1:
						doInvoke = true;
						value = fp[5];
						overrideReturnValueWithContext = false;
						break;
					case 2:
						doInvoke = true;
						value = fp[5];
						overrideReturnValueWithContext = false;
						break;
					case 3:
						doInvoke = true;
						value = fp[5];
						overrideReturnValueWithContext = false;
						break;
					case 5:
						classDef = fp[4][3];
						valueArr = classDef[10];
						sz = valueArr.length;
						values = PST$createNewArray(sz);
						i = 0;
						while (i < sz) {
							values[i] = valueArr[i];
							i += 1;
						}
						instance1 = [ec[17], classDef, values];
						ec[17] += 1;
						value = [12, instance1];
						doInvoke = true;
						overrideReturnValueWithContext = true;
						break;
					case 6:
						doInvoke = true;
						value = frame[7];
						overrideReturnValueWithContext = false;
						break;
					case 4:
						frame[1] = pc;
						frame[4] = valueStackSize;
						task[2] = frame;
						switch (fp[3]) {
							case 40:
								output = buildString(globalValues, buildBase64String(fp[5][1]), false);
								break;
							case 5:
								dictImpl1 = fp[5][1];
								keys = dictImpl1[4];
								sz = dictImpl1[2];
								valueArr = PST$createNewArray(sz);
								i = 0;
								while (i < sz) {
									valueArr[i] = keys[i];
									i += 1;
								}
								output = buildList(ec, valueArr, false, valueArr.length);
								break;
							case 9:
								dictImpl1 = fp[5][1];
								values = dictImpl1[5];
								sz = dictImpl1[2];
								valueArr = PST$createNewArray(sz);
								i = 0;
								while (i < sz) {
									valueArr[i] = values[i];
									i += 1;
								}
								output = buildList(ec, valueArr, false, valueArr.length);
								break;
							case 10:
								listImpl1 = fp[5][1];
								if (listImpl1[1] == listImpl1[2]) {
									List_expandCapacity(listImpl1);
								}
								output = globalValues[0];
								listImpl1[3][listImpl1[1]] = args[0];
								listImpl1[1] += 1;
								break;
							case 11:
								listImpl1 = fp[5][1];
								sz = listImpl1[1];
								valueArr = listImpl1[3];
								i = 0;
								while (i < sz) {
									valueArr[i] = null;
									i += 1;
								}
								listImpl1[1] = 0;
								output = VALUE_NULL;
								break;
							case 12:
								listImpl1 = fp[5][1];
								output = buildList(ec, listImpl1[3], true, listImpl1[1]);
								break;
							case 14:
								value16[0] = fp[5];
								value16[1] = args[0];
								args = value16;
								argc += 1;
								fp = ec[5][ec[3]["filter"]][1];
								doInvoke = true;
								overrideReturnValueWithContext = false;
								break;
							case 15:
								listImpl1 = fp[5][1];
								sz = listImpl1[1];
								int2 = 0;
								int3 = sz;
								if (argc > 1) {
									value = args[1];
									if (value[0] != 3) {
										errorId = 4;
										errorMsg = "starting index must be an integer.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
									int2 = value[1];
									if (int2 < 0) {
										int2 += sz;
									}
									if (int2 < 0 || int2 >= sz) {
										errorId = 8;
										errorMsg = "starting index out of range.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
								}
								if (argc == 3) {
									value = args[2];
									if (value[0] != 3) {
										errorId = 4;
										errorMsg = "end index must be an integer.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
									int3 = value[1];
									if (int3 < 0) {
										int3 += sz;
									}
									if (int3 < 0 || int3 >= sz) {
										errorId = 8;
										errorMsg = "end index out of range.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
								}
								valueArr = listImpl1[3];
								left = args[0];
								leftType = left[0];
								object1 = left[1];
								bool1 = false;
								output = globalValues[4][1];
								i = int2;
								while (i < int3) {
									if (isValueEqual(left, valueArr[i])) {
										output = buildInteger(globalValues, i);
										i += sz;
									}
									i += 1;
								}
								break;
							case 16:
								listImpl1 = fp[5][1];
								sz = listImpl1[1];
								int2 = sz - 1;
								int3 = -1;
								if (argc > 1) {
									value = args[1];
									if (value[0] != 3) {
										errorId = 4;
										errorMsg = "starting index must be an integer.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
									int2 = value[1];
									if (int2 < 0) {
										int2 += sz;
									}
									if (int2 < 0 || int2 >= sz) {
										errorId = 8;
										errorMsg = "starting index out of range.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
								}
								if (argc == 3) {
									value = args[2];
									if (value[0] != 3) {
										errorId = 4;
										errorMsg = "end index must be an integer.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
									int3 = value[1];
									if (int3 < -1) {
										int3 += sz;
									}
									if (int3 < -1 || int3 > sz - 1) {
										errorId = 8;
										errorMsg = "end index out of range.";
										return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
									}
								}
								valueArr = listImpl1[3];
								left = args[0];
								leftType = left[0];
								object1 = left[1];
								bool1 = false;
								output = globalValues[4][1];
								i = int2;
								while (i > int3) {
									if (isValueEqual(left, valueArr[i])) {
										output = buildInteger(globalValues, i);
										i -= int3;
									}
									i -= 1;
								}
								break;
							case 17:
								str1 = "";
								if (argc == 1) {
									value1 = args[0];
									if (value1[0] != 5) {
										return ThrowErrorImpl(task, 4, "list.join(sep) requires a string");
									}
									stringImpl1 = value1[1];
									if (stringImpl1[1]) {
										stringFlatten(stringImpl1);
									}
									str1 = stringImpl1[3];
								}
								output = List_join(globalValues, fp[5], str1);
								break;
							case 18:
								value16[0] = fp[5];
								value16[1] = args[0];
								args = value16;
								argc += 1;
								fp = ec[5][ec[3]["map"]][1];
								doInvoke = true;
								overrideReturnValueWithContext = false;
								break;
							case 19:
								listImpl1 = fp[5][1];
								sz = listImpl1[1] - 1;
								if (sz == -1) {
									errorId = 8;
									errorMsg = "Cannot pop from an empty list.";
									return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
								}
								output = listImpl1[3][sz];
								listImpl1[3][sz] = null;
								listImpl1[1] = sz;
								break;
							case 21:
								value16[0] = fp[5];
								value16[1] = args[0];
								value16[2] = null;
								if (args.length == 2) {
									value16[2] = args[1];
								}
								argc += 1;
								args = value16;
								fp = ec[5][ec[3]["reduce"]][1];
								doInvoke = true;
								overrideReturnValueWithContext = false;
								break;
							case 23:
								listImpl1 = fp[5][1];
								sz = listImpl1[1];
								int1 = sz >> 1;
								valueArr = listImpl1[3];
								j = sz - 1;
								i = 0;
								while (i < int1) {
									value = valueArr[i];
									valueArr[i] = valueArr[j];
									valueArr[j] = value;
									j -= 1;
									i += 1;
								}
								output = fp[5];
								break;
							case 22:
								value = args[0];
								if (value[0] != 3) {
									errorId = 4;
									errorMsg = "list.remove() requires a valid index integer.";
									return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
								}
								listImpl1 = fp[5][1];
								j = value[1];
								sz = listImpl1[1];
								if (j < 0) {
									j += sz;
								}
								if (j >= sz || j < 0) {
									errorId = 8;
									errorMsg = "list.remove() was given an index out of range.";
									return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
								}
								valueArr = listImpl1[3];
								output = valueArr[j];
								i = j + 1;
								while (i < sz) {
									valueArr[i - 1] = valueArr[i];
									i += 1;
								}
								sz -= 1;
								listImpl1[1] = sz;
								listImpl1[3][sz] = null;
								break;
							case 20:
								value16[0] = fp[5];
								if (args.length == 1) {
									value16[1] = args[0];
								}
								argc += 1;
								args = value16;
								fp = ec[5][ec[3]["sort"]][1];
								doInvoke = true;
								overrideReturnValueWithContext = false;
								break;
							case 39:
								value16[0] = fp[5];
								value16[1] = args[0];
								argc += 1;
								args = value16;
								fp = ec[5][ec[3]["sortK"]][1];
								doInvoke = true;
								overrideReturnValueWithContext = false;
								break;
							case 24:
								listImpl1 = fp[5][1];
								sz = listImpl1[1];
								intArray1 = PST$createNewArray(sz);
								i = 0;
								while (i < sz) {
									left = listImpl1[3][i];
									if (left[0] != 3) {
										return ThrowErrorImpl(task, 4, "Only lists of integers can be converted to byte buffers.");
									}
									intArray1[i] = 255 & left[1];
									i += 1;
								}
								output = [8, intArray1];
								intArray1 = null;
								break;
							case 29:
								value1 = args[0];
								if (value1[0] != 3) {
									return ThrowErrorImpl(task, 4, "string.getCodePoint() requires an integer index ");
								}
								i = value1[1];
								stringImpl1 = fp[5][1];
								sz = stringImpl1[0];
								if (i < 0) {
									i += sz;
								}
								if (i < 0 || i >= sz) {
									return ThrowErrorImpl(task, 8, "Index out of range.");
								}
								if (stringImpl1[1]) {
									stringFlatten(stringImpl1);
								}
								output = buildInteger(globalValues, stringImpl1[2][i]);
								break;
							case 30:
								output = stringUtil_changeCase(fp[5], false);
								break;
							case 32:
								value1 = args[0];
								if (value1[0] != 5) {
									return ThrowErrorImpl(task, 4, "string.split(sep) requires a string separator");
								}
								str1 = valueToHumanString(value1);
								valueArr = stringUtil_split(globalValues, fp[5], str1);
								sz = valueArr.length;
								output = [9, [ec[17], sz, sz, valueArr]];
								valueArr = null;
								ec[17] += 1;
								break;
							case 36:
								stringImpl1 = fp[5][1];
								sz = stringImpl1[0];
								valueArr = PST$createNewArray(sz);
								intArray1 = stringImpl1[2];
								i = 0;
								while (i < sz) {
									j = intArray1[i];
									if (j < 1200) {
										valueArr[i] = globalValues[3][j];
									} else {
										valueArr[i] = buildInteger(globalValues, j);
									}
									i += 1;
								}
								output = buildList(ec, valueArr, true, -1);
								break;
							case 37:
								output = stringUtil_trim(fp[5], true, true);
								break;
							case 38:
								output = stringUtil_changeCase(fp[5], true);
								break;
							default:
								frame[1] = pc;
								frame[4] = valueStackSize;
								task[2] = frame;
								return ExRes_HardCrash(task, "Corrupted method pointer.");
						}
						valueStack[valueStackSize] = output;
						valueStackSize += 1;
						break;
					default:
						frame[1] = pc;
						frame[4] = valueStackSize;
						task[2] = frame;
						return ExRes_HardCrash(task, "support for this function pointer type is not done yet.");
				}
				if (doInvoke) {
					frame[1] = pc;
					frame[4] = valueStackSize;
					task[2] = frame;
					fn = fp[4];
					nextFrame = task[6];
					if (nextFrame != null) {
						task[6] = nextFrame[0];
					} else {
						nextFrame = [null, 0, null, 0, 0, 0, null, null, false, null, false];
					}
					nextFrame[0] = frame;
					nextFrame[1] = fn[2] - 1;
					nextFrame[2] = args;
					nextFrame[3] = argc;
					nextFrame[4] = valueStackSize;
					nextFrame[5] = valueStackSize;
					nextFrame[6] = {};
					nextFrame[7] = value;
					nextFrame[8] = overrideReturnValueWithContext;
					nextFrame[9] = null;
					frame = nextFrame;
					task[2] = frame;
					pc = frame[1];
					locals = frame[6];
				}
				break;
			case 22:
				// OP_INDEX;
				valueStackSize -= 1;
				left = valueStack[valueStackSize - 1];
				right = valueStack[valueStackSize];
				switch (left[0] * 16 + right[0]) {
					case 83:
						stringImpl1 = left[1];
						if (stringImpl1[1]) {
							stringFlatten(stringImpl1);
						}
						sz = stringImpl1[0];
						i = right[1];
						bool1 = false;
						if (i < 0) {
							i += sz;
							if (i < 0) {
								bool1 = true;
							}
						} else if (i >= sz) {
							bool1 = true;
						}
						if (bool1) {
							errorId = 8;
							errorMsg = "String index out of range.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						intArray1 = PST$createNewArray(1);
						intArray1[0] = stringImpl1[2][i];
						str1 = stringImpl1[3].substring(i, i + 1);
						valueStack[valueStackSize - 1] = [5, [1, false, intArray1, str1, null, null]];
						break;
					case 147:
						listImpl1 = left[1];
						i = right[1];
						sz = listImpl1[1];
						bool1 = false;
						if (i < 0) {
							i += sz;
							if (i < 0) {
								bool1 = true;
							}
						} else if (i >= sz) {
							bool1 = true;
						}
						if (bool1) {
							errorId = 8;
							errorMsg = "List index out of range.";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						valueStack[valueStackSize - 1] = listImpl1[3][i];
						break;
					case 163:
						dictImpl1 = left[1];
						i = right[1];
						if (dictImpl1[1] == 3) {
							if (dictImpl1[6][i] !== undefined) {
								bool1 = false;
								valueStack[valueStackSize - 1] = dictImpl1[5][dictImpl1[6][i]];
							} else {
								bool1 = true;
							}
						} else {
							bool1 = true;
						}
						if (bool1) {
							errorId = 6;
							errorMsg = "Dictionary does not have that key";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						break;
					case 165:
						dictImpl1 = left[1];
						stringImpl1 = right[1];
						if (stringImpl1[1]) {
							stringFlatten(stringImpl1);
						}
						str1 = stringImpl1[3];
						if (dictImpl1[1] == 5) {
							if (dictImpl1[7][str1] !== undefined) {
								bool1 = false;
								valueStack[valueStackSize - 1] = dictImpl1[5][dictImpl1[7][str1]];
							} else {
								bool1 = true;
							}
						} else {
							bool1 = true;
						}
						if (bool1) {
							errorId = 6;
							errorMsg = "Dictionary does not have that key";
							return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
						}
						break;
					default:
						frame[1] = pc;
						frame[4] = valueStackSize;
						task[2] = frame;
						switch (left[0]) {
							case 5:
								errorMsg = "Strings must be indexed with an integer.";
								break;
							case 9:
								errorMsg = "Lists must be indexed with an integer.";
								break;
							default:
								errorMsg = "Cannot index into this type";
								break;
						}
						errorId = 4;
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 23:
				// OP_INT_INCR;
				i = valueStackSize - 1;
				value = valueStack[i];
				if (value[0] != 3) {
					errorId = 9;
					errorMsg = "Cannot increment/decrement non-integer";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				valueStack[i] = buildInteger(globalValues, row[2] + value[1]);
				break;
			case 24:
				// OP_JUMP;
				pc += row[2];
				break;
			case 26:
				// OP_NEGATIVE_SIGN;
				value = valueStack[valueStackSize - 1];
				switch (value[0]) {
					case 3:
						i = -value[1];
						if (i < 1200 && -i > 1200) {
							if (i < 0) {
								value = globalValues[3][i];
							} else {
								value = globalValues[3][i];
							}
						} else {
							value = [3, i];
						}
						break;
					case 4:
						float1 = value[1];
						if (float1 != 0) {
							value = [4, -float1];
						}
						break;
					default:
						errorId = 4;
						errorMsg = "Cannot apply a negative sign to this type.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				valueStack[valueStackSize - 1] = value;
				break;
			case 27:
				// OP_POP;
				valueStackSize -= 1;
				break;
			case 28:
				// OP_POP_AND_JUMP_IF_FALSE;
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				if (!value[1]) {
					pc += row[2];
				}
				break;
			case 29:
				// OP_POP_AND_JUMP_IF_TRUE;
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				if (value[1]) {
					pc += row[2];
				}
				break;
			case 30:
				// OP_POP_IF_FALSE_OR_JUMP;
				value = valueStack[valueStackSize - 1];
				if (value[0] == 2 && !value[1]) {
					valueStackSize -= 1;
				} else {
					pc += row[2];
				}
				break;
			case 31:
				// OP_POP_IF_NULL_OR_JUMP;
				value = valueStack[valueStackSize - 1];
				if (value[0] == 1) {
					valueStackSize -= 1;
				} else {
					pc += row[2];
				}
				break;
			case 32:
				// OP_POP_IF_TRUE_OR_JUMP;
				value = valueStack[valueStackSize - 1];
				if (value[0] == 2 && value[1]) {
					valueStackSize -= 1;
				} else {
					pc += row[2];
				}
				break;
			case 33:
				// OP_PUSH_ARG;
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = frame[2][row[2]];
				valueStackSize += 1;
				break;
			case 34:
				// OP_PUSH_ARG_IF_PRESENT;
				if (row[2] < frame[3]) {
					if (valueStackSize == valueStackCapacity) {
						valueStack = increaseValueStackCapacity(task);
						valueStackCapacity = valueStack.length;
					}
					valueStack[valueStackSize] = frame[2][row[2]];
					valueStackSize += 1;
					pc += row[3];
				}
				break;
			case 35:
				// OP_PUSH_BASE_CTOR;
				value = ec[6][row[2]][4];
				fp = FunctionPointer_cloneWithNewType(value[1], 6);
				row[8] = [11, fp];
				row[0] = 44;
				pc -= 1;
				break;
			case 36:
				// OP_PUSH_BOOL;
				row[8] = globalValues[2];
				if (row[2] == 1) {
					row[8] = globalValues[1];
				}
				row[0] = 44;
				pc -= 1;
				break;
			case 37:
				// OP_PUSH_CLASS_REF;
				classDef = ec[6][row[2]];
				if (classDef[6]) {
					row[8] = classDef[13];
					row[0] = 44;
					pc -= 1;
				} else {
					classDef[6] = true;
					if (classDef[5] > 0) {
						frame[1] = pc;
						frame[4] = valueStackSize;
						task[2] = frame;
						pc = ec[4][classDef[5]][2] - 1;
						frame = [frame, pc, PST$createNewArray(0), 0, valueStackSize, valueStackSize, {}, classDef[13], true, null, false];
						locals = frame[6];
						task[2] = frame;
					} else {
						pc -= 1;
					}
				}
				break;
			case 38:
				// OP_PUSH_FLOAT;
				if (row[1].length == 1) {
					i = row[2];
					if (i >= 0 && i <= 4) {
						value = globalValues[9][i];
					} else {
						value = buildFloat(i * 0.25);
					}
				} else {
					value = buildFloat(parseFloat(row[5]));
				}
				row[8] = value;
				row[0] = 44;
				pc -= 1;
				break;
			case 39:
				// OP_PUSH_FUNC_PTR;
				row[8] = ec[5][row[2]];
				row[0] = 44;
				pc -= 1;
				break;
			case 40:
				// OP_PUSH_INT;
				row[8] = buildInteger(globalValues, row[2]);
				row[0] = 44;
				pc -= 1;
				break;
			case 41:
				// OP_PUSH_NULL;
				row[8] = globalValues[0];
				row[0] = 44;
				pc -= 1;
				break;
			case 42:
				// OP_PUSH_STRING;
				row[8] = buildString(globalValues, row[5], true);
				row[0] = 44;
				pc -= 1;
				break;
			case 43:
				// OP_PUSH_THIS;
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = frame[7];
				valueStackSize += 1;
				break;
			case 44:
				// OP_PUSH_VALUE;
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = row[8];
				valueStackSize += 1;
				break;
			case 45:
				// OP_PUSH_VAR;
				name = row[5];
				if (locals[name] !== undefined) {
					if (valueStackSize == valueStackCapacity) {
						valueStack = increaseValueStackCapacity(task);
						valueStackCapacity = valueStack.length;
					}
					valueStack[valueStackSize] = locals[name];
					valueStackSize += 1;
				} else {
					errorId = 1;
					errorMsg = ["The variable '", row[5], "' has not been assigned a value."].join('');
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				break;
			case 46:
				// OP_RETURN;
				if (ec[10][pc] != null) {
					frame[1] = pc;
					frame[4] = valueStackSize;
					task[2] = frame;
					return ExRes_HardCrash(task, "TODO: returns from within try/catch/finally blocks");
				}
				if (frame[8]) {
					valueStack[valueStackSize - 1] = frame[7];
				}
				nextFrame = frame[0];
				frame[0] = task[6];
				task[6] = frame;
				frame[7] = null;
				frame = nextFrame;
				task[2] = frame;
				if (frame == null) {
					return ExRes_Done(task);
				}
				locals = frame[6];
				pc = frame[1];
				break;
			case 47:
				// OP_SLICE;
				i = row[2];
				valueStackSize -= 1;
				if ((i & 4) > 0) {
					int3 = valueStack[valueStackSize][1];
					if (int3 == 0) {
						errorId = 4;
						errorMsg = "Cannot use 0 as a step distance for a slice.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
					}
					valueStackSize -= 1;
				} else {
					int3 = 1;
				}
				if ((i & 2) > 0) {
					int2 = valueStack[valueStackSize][1];
					valueStackSize -= 1;
					bool2 = true;
				} else {
					bool2 = false;
				}
				if ((i & 1) > 0) {
					int1 = valueStack[valueStackSize][1];
					valueStackSize -= 1;
					bool1 = true;
				} else {
					bool1 = false;
				}
				value = valueStack[valueStackSize];
				if (value[0] == 5) {
					bool3 = true;
					stringImpl1 = value[1];
					if (stringImpl1[1]) {
						stringFlatten(stringImpl1);
					}
					sz = stringImpl1[0];
				} else if (value[0] == 9) {
					bool3 = false;
					listImpl1 = value[1];
					sz = listImpl1[1];
				} else {
					errorId = 4;
					errorMsg = "Slicing can only be performed on strings or lists.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				if (bool1) {
					if (int1 < 0) {
						int1 += sz;
					}
					if (int1 < 0 || int1 >= sz) {
						errorId = 8;
						errorMsg = "Start index is out of range.";
						return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
					}
				}
				if (bool2 && int2 < 0) {
					int2 += sz;
				}
				if (bool1) {
					if (bool2) {
						if (int3 > 0) {
							if (int1 <= int2) {
								leftType = int1;
								rightType = int2;
								if (rightType > sz) {
									rightType = sz;
								}
							} else {
								leftType = 0;
								rightType = 0;
							}
						} else if (int1 <= int2) {
							leftType = 0;
							rightType = 0;
						} else {
							leftType = int1;
							rightType = int2;
							if (rightType < -1) {
								rightType = -1;
							}
						}
					} else if (int3 > 0) {
						leftType = int1;
						rightType = sz;
					} else {
						leftType = int1;
						rightType = -1;
					}
				} else if (bool2) {
					if (int3 > 0) {
						leftType = 0;
						rightType = int2;
						if (rightType > sz) {
							rightType = sz;
						}
					} else {
						leftType = sz - 1;
						rightType = int2;
						if (rightType < -1) {
							rightType = -1;
						}
					}
				} else if (int3 > 0) {
					leftType = 0;
					rightType = sz;
				} else {
					leftType = sz - 1;
					rightType = -1;
				}
				if (bool3) {
					intList = [];
					intArray1 = stringImpl1[2];
					if (int3 > 0) {
						i = leftType;
						while (i < rightType) {
							intList.push(intArray1[i]);
							i += int3;
						}
					} else {
						i = leftType;
						while (i > rightType) {
							intList.push(intArray1[i]);
							i += int3;
						}
					}
					output = createStringFromUnicodeArray(globalValues, [...(intList)], false);
				} else {
					valueList = [];
					valueArr = listImpl1[3];
					if (int3 > 0) {
						i = leftType;
						while (i < rightType) {
							valueList.push(valueArr[i]);
							i += int3;
						}
					} else {
						i = leftType;
						while (i > rightType) {
							valueList.push(valueArr[i]);
							i += int3;
						}
					}
					sz = valueList.length;
					output = [9, [ec[17], sz, sz, [...(valueList)]]];
				}
				valueStack[valueStackSize] = output;
				valueStackSize += 1;
				break;
			case 48:
				// OP_SPECIAL_ACTION;
				output = globalValues[0];
				switch (row[2]) {
					case 6:
						valueStackSize -= 2;
						left = valueStack[valueStackSize];
						right = valueStack[valueStackSize + 1];
						leftType = left[0];
						rightType = right[0];
						if (leftType != rightType) {
							if (leftType == 3 && rightType == 4) {
								float1 = left[1] - right[1];
							} else if (leftType == 4 && rightType == 3) {
								float1 = left[1] - right[1];
							} else {
								float1 = 0.0 + leftType - rightType;
							}
						} else if (leftType == 3) {
							float1 = 0.0 + left[1] - right[1];
						} else if (leftType == 4) {
							float1 = left[1] - right[1];
						} else if (leftType == 5) {
							stringImpl1 = left[1];
							stringImpl2 = right[1];
							if (stringImpl1[1]) {
								stringFlatten(stringImpl1);
							}
							if (stringImpl2[1]) {
								stringFlatten(stringImpl2);
							}
							sz = stringImpl1[0];
							int2 = stringImpl2[0];
							if (int2 < sz) {
								sz = int2;
							}
							int3 = 0;
							i = 0;
							while (i < sz) {
								int1 = stringImpl1[2][i];
								int2 = stringImpl2[2][i];
								if (int1 != int2) {
									int3 = int1 - int2;
									i += sz;
								}
								i += 1;
							}
							if (int3 == 0) {
								int3 = stringImpl2[0] - stringImpl1[0];
							}
							float1 = 0.0 + int3;
						} else {
							float1 = 0.0;
						}
						if (float1 == 0) {
							output = globalValues[7];
						} else if (float1 < 0) {
							output = globalValues[4][1];
						} else {
							output = globalValues[8];
						}
						break;
					case 7:
						output = buildFloat(Math.random());
						break;
					case 3:
						valueStackSize -= 1;
						output = Sort_end(valueStack[valueStackSize][1]);
						break;
					case 4:
						valueStackSize -= 2;
						output = VALUE_FALSE;
						if (Sort_getNextCmp(valueStack[valueStackSize], valueStack[valueStackSize + 1])) {
							output = VALUE_TRUE;
						}
						break;
					case 5:
						valueStackSize -= 2;
						value = valueStack[valueStackSize];
						bool1 = valueStack[valueStackSize + 1][1];
						output = Sort_proceedWithCmpResult(value, bool1);
						break;
					case 2:
						valueStackSize -= 2;
						output = Sort_start(valueStack[valueStackSize], valueStack[valueStackSize + 1]);
						break;
					case 1:
						float1 = (Date.now ? Date.now() : new Date().getTime()) / 1000.0;
						if (row[3] == 1) {
							output = buildFloat(float1);
						} else {
							output = buildInteger(globalValues, Math.floor(float1));
						}
						break;
				}
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = output;
				valueStackSize += 1;
				break;
			case 49:
				// OP_STACK_DO_SI_DO_4A;
				i = valueStackSize - 4;
				j = i + 2;
				value = valueStack[i];
				valueStack[i] = valueStack[j];
				valueStack[j] = value;
				value = valueStack[i + 1];
				valueStack[i + 1] = valueStack[j + 1];
				valueStack[j + 1] = value;
				break;
			case 50:
				// OP_STACK_DO_SI_DUP_1;
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				i = valueStackSize - 2;
				valueStackSize += 1;
				valueStack[i + 2] = valueStack[i];
				valueStack[i] = valueStack[i + 1];
				break;
			case 51:
				// OP_STACK_DO_SI_DUP_2;
				i = valueStackSize - 3;
				value = valueStack[i];
				valueStack[i] = valueStack[i + 1];
				valueStack[i + 1] = valueStack[i + 2];
				valueStack[i + 2] = value;
				break;
			case 52:
				// OP_STACK_DUPLICATE;
				if (valueStackSize == valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = valueStack[valueStackSize - 1];
				valueStackSize += 1;
				break;
			case 53:
				// OP_STACK_DUPLICATE_2;
				if (valueStackSize + 2 >= valueStackCapacity) {
					valueStack = increaseValueStackCapacity(task);
					valueStackCapacity = valueStack.length;
				}
				valueStack[valueStackSize] = valueStack[valueStackSize - 2];
				valueStack[valueStackSize + 1] = valueStack[valueStackSize - 1];
				valueStackSize += 2;
				break;
			case 54:
				// OP_SWITCH_ADD;
				if (row[2] < 0) {
					i = 1;
					while (i < row[1].length) {
						switchIntLookup[row[1][i + 1]] = row[1][i];
						i += 2;
					}
				} else {
					switchStrLookup[row[5]] = row[2];
				}
				break;
			case 55:
				// OP_SWITCH_BUILD;
				if (row[3] == 1) {
					switchIntLookup = {};
					switchStrLookup = null;
				} else {
					switchIntLookup = null;
					switchStrLookup = {};
				}
				break;
			case 56:
				// OP_SWITCH_FINALIZE;
				pc -= row[1][0];
				row = byteCode[pc];
				if (switchIntLookup != null) {
					ec[13][pc] = switchIntLookup;
					row[0] = 57;
				} else {
					ec[14][pc] = switchStrLookup;
					row[0] = 58;
				}
				pc -= 1;
				break;
			case 57:
				// OP_SWITCH_INT;
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				i = value[1];
				switchIntLookup = ec[13][pc];
				if (switchIntLookup[i] !== undefined) {
					pc += switchIntLookup[i];
				} else {
					pc += row[2];
				}
				break;
			case 58:
				// OP_SWITCH_STRING;
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				stringImpl1 = value[1];
				if (stringImpl1[1]) {
					stringFlatten(stringImpl1);
				}
				str1 = stringImpl1[3];
				switchStrLookup = ec[14][pc];
				if (switchStrLookup[str1] !== undefined) {
					pc += switchStrLookup[str1];
				} else {
					pc += row[2];
				}
				break;
			case 59:
				// OP_THROW;
				valueStackSize -= 1;
				value = valueStack[valueStackSize];
				bool1 = false;
				if (value[0] == 12) {
					instance1 = value[1];
					int1 = 0;
					i = 1;
					while (i < ec[6].length && int1 == 0) {
						if (ec[6][i][3] == "Exception") {
							int1 = i;
						}
						i += 1;
					}
					classDef = instance1[1];
					while (classDef != null) {
						if (classDef[0] == int1) {
							bool1 = true;
						}
						classDef = classDef[2];
					}
				}
				if (!bool1) {
					errorId = 4;
					errorMsg = "Only Exception instances can be thrown.";
					return ThrowError(task, frame, pc, valueStackSize, errorId, errorMsg);
				}
				frame[1] = pc;
				frame[4] = valueStackSize;
				task[2] = frame;
				instance1[2][instance1[1][8]["trace"]] = generateStackTrace(task);
				if (bubbleException(task, value)) {
					return ExRes_HardCrash(task, getExceptionMessage(value, true));
				}
				frame = task[2];
				pc = frame[1];
				locals = frame[6];
				break;
			case 61:
				// OP_TRY_CATCH_ROUTER;
				value = frame[9];
				frame[10] = exceptionCatcherRouteException(value, row[1], intBuffer16);
				pc += intBuffer16[0];
				if (!frame[10]) {
					if (valueStackSize == valueStackCapacity) {
						valueStack = increaseValueStackCapacity(task);
						valueStackCapacity = valueStack.length;
					}
					valueStack[valueStackSize] = value;
					valueStackSize += 1;
					frame[9] = null;
				}
				break;
			case 60:
				// OP_TRY_FINALLY_END;
				if (frame[9] != null) {
					value = frame[9];
					if (frame[10]) {
						frame[1] = pc;
						frame[4] = valueStackSize;
						task[2] = frame;
						task[2] = frame;
						if (bubbleException(task, value)) {
							return ExRes_HardCrash(task, getExceptionMessage(value, true));
						}
						frame = task[2];
						pc = frame[1];
						locals = frame[6];
					}
				}
				break;
			default:
				frame[1] = pc;
				frame[4] = valueStackSize;
				task[2] = frame;
				return ExRes_HardCrash(task, "INVALID OP CODE: " + (row[0] + ''));
		}
		pc += 1;
	}
};

let Sort_buildTaskList = function(start, length, items, output, tasks) {
	if (length == 1) {
		let node = [items[start], output[start], null];
		let task = [true, null, null, node, node, null, false, null];
		tasks.push(task);
		return task;
	}
	if (length == 2) {
		let left = [items[start], output[start], null];
		let right = [items[start + 1], output[start + 1], null];
		let task2 = [false, left, right, null, null, null, false, null];
		tasks.push(task2);
		return task2;
	}
	let half = length >> 1;
	let leftTask = Sort_buildTaskList(start, half, items, output, tasks);
	let rightTask = Sort_buildTaskList(start + half, length - half, items, output, tasks);
	let taskMerge = [false, null, null, null, null, null, false, null];
	leftTask[5] = taskMerge;
	leftTask[6] = true;
	rightTask[5] = taskMerge;
	tasks.push(taskMerge);
	return taskMerge;
};

let Sort_end = function(state) {
	let arr = state[5];
	let items = (arr[1])[3];
	let walker = state[0];
	let i = 0;
	while (walker != null) {
		items[i] = walker[1];
		walker = walker[2];
		i += 1;
	}
	return arr;
};

let Sort_getNextCmp = function(sortStateValue, pairValue) {
	let pair = (pairValue[1])[3];
	let o = sortStateValue[1];
	let task = o[4];
	let keepRunning = true;
	while (keepRunning) {
		if (task[0]) {
			if (task[5] == null) {
				o[0] = task[3];
				keepRunning = false;
				return false;
			}
			if (task[6]) {
				task[5][1] = task[3];
			} else {
				task[5][2] = task[3];
			}
			task = task[7];
			o[4] = task;
			keepRunning = task != null;
		} else {
			if (task[1] != null && task[2] != null) {
				pair[0] = task[1][0];
				pair[1] = task[2][0];
				return true;
			}
			if (task[1] == null && task[2] == null) {
				task[0] = true;
			} else {
				let mergeWinner = null;
				if (task[1] == null) {
					mergeWinner = task[2];
					task[2] = mergeWinner[2];
					mergeWinner[2] = null;
				} else {
					mergeWinner = task[1];
					task[1] = mergeWinner[2];
					mergeWinner[2] = null;
				}
				if (task[3] == null) {
					task[3] = mergeWinner;
					task[4] = mergeWinner;
				} else {
					task[4][2] = mergeWinner;
					task[4] = mergeWinner;
				}
			}
		}
	}
	return false;
};

let Sort_proceedWithCmpResult = function(stateVal, isSwap) {
	let state = stateVal[1];
	let task = state[4];
	let winner = null;
	if (isSwap) {
		winner = task[2];
		task[2] = winner[2];
	} else {
		winner = task[1];
		task[1] = winner[2];
	}
	winner[2] = null;
	if (task[3] == null) {
		task[3] = winner;
	} else {
		task[4][2] = winner;
	}
	task[4] = winner;
	return stateVal;
};

let Sort_start = function(valueList, mirrorList) {
	if (mirrorList[0] == 1) {
		mirrorList = valueList;
	}
	let values = valueList[1];
	let sz = values[1];
	let items = values[3];
	let mirror = (mirrorList[1])[3];
	let o = [null, null, null, values[1] < 2, null, mirrorList];
	if (o[3]) {
		if (items.length == 1) {
			o[0] = [items[0], mirror[0], null];
		}
	} else {
		let tasks = [];
		Sort_buildTaskList(0, sz, items, mirror, tasks);
		let i = 1;
		while (i < tasks.length) {
			tasks[i - 1][7] = tasks[i];
			i += 1;
		}
		o[4] = tasks[0];
	}
	return [14, o];
};

let stringFlatten = function(s) {
	let ucharsBuilder = PST$createNewArray(s[0]);
	let ucharOffset = 0;
	let strBuilder = [];
	let q = [];
	q.push(s);
	let current = null;
	let i = 0;
	let currentLen = 0;
	let currentUChars = null;
	while (q.length > 0) {
		current = q.pop();
		if (current[1]) {
			q.push(current[5]);
			q.push(current[4]);
		} else {
			currentUChars = current[2];
			currentLen = current[0];
			i = 0;
			while (i < currentLen) {
				ucharsBuilder[i + ucharOffset] = currentUChars[i];
				i += 1;
			}
			ucharOffset += currentLen;
			strBuilder.push(current[3]);
		}
	}
	s[1] = false;
	s[4] = null;
	s[5] = null;
	s[3] = strBuilder.join("");
	s[2] = ucharsBuilder;
};

let stringUtil_changeCase = function(orig, isUpper) {
	let changes = false;
	let si = orig[1];
	if (si[1]) {
		stringFlatten(si);
	}
	let s = si[3];
	let s2 = null;
	if (isUpper) {
		s2 = s.toUpperCase();
	} else {
		s2 = s.toLowerCase();
	}
	if (s2 == s) {
		return orig;
	}
	return [5, buildAsciiStringImpl(s2)];
};

let stringUtil_split = function(g, str, sep) {
	let si = str[1];
	if (si[1]) {
		stringFlatten(si);
	}
	let values = si[3].split(sep);
	let sz = values.length;
	let o = PST$createNewArray(sz);
	let i = 0;
	while (i < sz) {
		o[i] = buildString(g, values[i], false);
		i += 1;
	}
	return o;
};

let stringUtil_trim = function(str, front, back) {
	let strimpl = str[1];
	let start = 0;
	let length = strimpl[0];
	if (length == 0) {
		return str;
	}
	if (strimpl[1]) {
		stringFlatten(strimpl);
	}
	let c = 0;
	let uchars = strimpl[2];
	while (back && length > 0) {
		c = uchars[length - 1];
		switch (c) {
			case 9:
				length -= 1;
				break;
			case 10:
				length -= 1;
				break;
			case 13:
				length -= 1;
				break;
			case 32:
				length -= 1;
				break;
			default:
				back = false;
				break;
		}
	}
	while (front && length > 0) {
		c = uchars[start];
		switch (c) {
			case 9:
				length -= 1;
				start += 1;
				break;
			case 10:
				length -= 1;
				start += 1;
				break;
			case 13:
				length -= 1;
				start += 1;
				break;
			case 32:
				length -= 1;
				start += 1;
				break;
			default:
				front = false;
				break;
		}
	}
	if (length == strimpl[0]) {
		return str;
	}
	let newChars = PST$createNewArray(length);
	let i = 0;
	while (i < length) {
		newChars[i] = uchars[i + start];
		i += 1;
	}
	let o = [length, false, newChars, strimpl[3].substring(start, start + length), null, null];
	return [5, o];
};

let ThrowError = function(task, frame, pc, valueStackSize, errId, msg) {
	frame[1] = pc;
	frame[4] = valueStackSize;
	task[2] = frame;
	return ThrowErrorImpl(task, errId, msg);
};

let ThrowErrorImpl = function(task, errId, msg) {
	let frame = task[2];
	if (frame[4] + 5 < task[5].length) {
		increaseValueStackCapacity(task);
	}
	let ec = task[1];
	let g = ec[1];
	let throwFunc = ec[4][ec[3]["thrw"]];
	let args = PST$createNewArray(2);
	args[0] = buildInteger(g, errId);
	args[1] = buildString(g, msg, false);
	task[2] = [frame, throwFunc[2], args, 2, frame[4], frame[4], {}, null, false, null, false];
	return [5, task, 0, null, null];
};

let tryGetNameId = function(names, s) {
	if (names[s] !== undefined) {
		return names[s];
	}
	return -1;
};

let valueArrayIncreaseCapacity = function(oldArr) {
	let oldCapacity = oldArr.length;
	let newCapacity = oldCapacity * 2;
	if (newCapacity < 8) {
		newCapacity = 8;
	}
	let newArr = PST$createNewArray(newCapacity);
	let i = 0;
	while (i < oldCapacity) {
		newArr[i] = oldArr[i];
		i += 1;
	}
	return newArr;
};

let valueToHumanString = function(value) {
	switch (value[0]) {
		case 1:
			return "null";
		case 2:
			let b = value[1];
			if (b) {
				return "true";
			}
			return "false";
		case 3:
			return value[1] + '';
		case 4:
			let floatStr = value[1] + '';
			if (!(floatStr.indexOf(".") != -1)) {
				floatStr = floatStr + ".0";
			}
			return floatStr;
		case 5:
			let strImpl = value[1];
			if (strImpl[1]) {
				stringFlatten(strImpl);
			}
			return strImpl[3];
		case 12:
			let inst = value[1];
			return ["Instance<", inst[1][3], ":", inst[0] + '', ">"].join('');
		default:
			break;
	}
	return "TODO: to string for type: " + (value[0] + '');
};

return [bubbleException, buildAsciiStringImpl, buildBase64String, buildFloat, buildFunctionFromInfo, buildInteger, buildList, buildString, convertToStringImpl, createClassInfo, createMainTask, createNewTask, createStringFromUnicodeArray, DictImpl_ensureCapacity, exceptionCatcherRouteException, ExRes_Done, ExRes_HardCrash, ExRes_Suspend, finalizeExecutionContext, FunctionPointer_cloneWithNewType, generateNameLookup, generateStackTrace, generateTryDescriptors, getExceptionMessage, getGlobalsFromTask, increaseValueStackCapacity, injectNameLookup, isValueEqual, List_add, List_expandCapacity, List_get, List_join, List_pop, List_removeAt, List_set, new_ByteCodeRow, new_ExecutionContext, new_ExecutionResult, new_GlobalValues, ParseRaw_entitiesSection_classMemberResolver, ParseRaw_entitiesSection_parseClasses, ParseRaw_entitiesSection_parseEnums, ParseRaw_entitiesSection_parseFunctions, ParseRaw_parseEntityData, ParseRaw_parseMetadata, ParseRaw_parseStringData, ParseRaw_parseTokenData, ParseRaw_popByteCodeRows, ParseRaw_popBytes, ParseRaw_popFixedLenString, ParseRaw_popInt, ParseRaw_popLenString, ParseRaw_popSingleByte, ParseRawData, PUBLIC_getApplicationContextFromTask, PUBLIC_getExecutionContextError, PUBLIC_getTaskResultError, PUBLIC_getTaskResultSleepAmount, PUBLIC_getTaskResultStatus, PUBLIC_initializeExecutionContext, PUBLIC_listValueAdd, PUBLIC_requestTaskSuspension, PUBLIC_startMainTask, PUBLIC_unwrapInteger, PUBLIC_unwrapNativeHandle, PUBLIC_valueToString, PUBLIC_wrapBoolean, PUBLIC_wrapInteger, PUBLIC_wrapNativeHandle, PUBLIC_wrapString, RunInterpreter, RunInterpreterImpl, Sort_buildTaskList, Sort_end, Sort_getNextCmp, Sort_proceedWithCmpResult, Sort_start, stringFlatten, stringUtil_changeCase, stringUtil_split, stringUtil_trim, ThrowError, ThrowErrorImpl, tryGetNameId, valueArrayIncreaseCapacity, valueToHumanString];
})();

  //
let newEngineContextBuilder = (name, ver) => {
  let extensions = {};
  let registerExtension = (name, fn) => {
    extensions[name] = fn;
    return o;
  };

  let lockConfiguration = () => {
    let finalizedExtensions = Object.freeze({ ...extensions });

    let buildRuntimeTask = (ec, taskObj) => {
      return Object.freeze({
        resume: () => {
          let resultRaw = RunInterpreter(taskObj);
          return buildTaskResult(resultRaw);
        },
      })
    };

    let buildTaskResult = rawResult => {
      let type = PUBLIC_getTaskResultStatus(rawResult);
      let err = PUBLIC_getTaskResultError(rawResult);
      let sleepAmt = PUBLIC_getTaskResultSleepAmount(rawResult);
      return Object.freeze({
        isError: () => type === 2,
        isSuccess: () => type === 1,
        isSuspend: () => type === 3,
        isTimedSleep: () => type === 4,
        getSleepAmountMillis: () => type === 4 ? sleepAmt : null,
        getError: () => err ? [...err] : null,
      });
    };

    let engine = {
      createAdaptiveCompilation: (rootModuleId) => {
        throw new Error("Not implemented");
      },
      createRuntimeContext: (byteArrOrBase64, cliArgs, appCtx) => {
        if (typeof byteArrOrBase64 === 'string') {
          throw new Error("base64 Not implemented");
        }
        if (!(byteArrOrBase64 instanceof Uint8Array)) {
          throw new Error("Invalid byte code input.");
        }

        let byteCodeBytes = byteArrOrBase64;

        let execCtx = PUBLIC_initializeExecutionContext(byteCodeBytes, finalizedExtensions, appCtx);
        let initErr = PUBLIC_getExecutionContextError(execCtx);
        if (initErr) throw new Error("Could not initialized executable: " + initErr);
        let args = (cliArgs || []).map(v => `${v}`);
        let mainTaskRaw = createMainTask(execCtx, args);
        let mainTask = buildRuntimeTask(execCtx, mainTaskRaw);

        return Object.freeze({
          getMainTask: () => mainTask,
          startMainTask: () => mainTask.resume(),
        });
      },
    };

    let o = Object.freeze({ ...engine });
    return o;
  };

  let o = Object.freeze({
    registerExtension,
    lockConfiguration,
  });
  return o;
};


  return Object.freeze({
    newEngineContextBuilder,
    task: Object.freeze({
        suspendTask: task => { PUBLIC_requestTaskSuspension(task, false, 0); },
        sleepTask: (task, millis) => { PUBLIC_requestTaskSuspension(task, true, millis); },
    }),
    runtimeValueConverter: Object.freeze({
      toReadableString: PUBLIC_valueToString,
      wrapNativeHandle: PUBLIC_wrapNativeHandle,
      wrapBoolean: PUBLIC_wrapBoolean,
      wrapInteger: PUBLIC_wrapInteger,
      wrapString: PUBLIC_wrapString,
      unwrapNativeHandle: PUBLIC_unwrapNativeHandle,
      unwrapInteger: PUBLIC_unwrapInteger,
      unwrapAppContext: PUBLIC_getApplicationContextFromTask,
      listAdd: PUBLIC_listValueAdd,
      // listLength: PUBLIC_listLength,
      // listGet: PUBLIC_listGet,
    }),
  });
})();

  PlexiOS.HtmlUtil.registerComponent('CommonScript_0_1_0', () => CommonScript);
})();
