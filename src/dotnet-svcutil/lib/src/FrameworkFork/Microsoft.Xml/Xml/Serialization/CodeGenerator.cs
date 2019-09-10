﻿//------------------------------------------------------------------------------
// <copyright file="CodeGenerator.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">YoussefM</owner>                                                                
//------------------------------------------------------------------------------

namespace Microsoft.Xml.Serialization {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Xml;
    //using Microsoft.Xml.Serialization.Configuration;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.IO;
    using System.Security;
    using System.Text.RegularExpressions;
    using System.Diagnostics;

    internal class CodeGenerator {

        // [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Method does validation only without any user input")]
        internal static bool IsValidLanguageIndependentIdentifier(string ident) { return Microsoft.CodeDom.Compiler.CodeGenerator.IsValidLanguageIndependentIdentifier(ident); }
        // [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Method does validation only without any user input")]
        internal static void ValidateIdentifiers(Microsoft.CodeDom.CodeObject e) { Microsoft.CodeDom.Compiler.CodeGenerator.ValidateIdentifiers(e); }
        internal static BindingFlags InstancePublicBindingFlags = BindingFlags.Instance | BindingFlags.Public;
        internal static BindingFlags InstanceBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        internal static BindingFlags StaticBindingFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
        internal static MethodAttributes PublicMethodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig;
        internal static MethodAttributes PublicOverrideMethodAttributes = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig;
        internal static MethodAttributes ProtectedOverrideMethodAttributes = MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig;
        internal static MethodAttributes PrivateMethodAttributes = MethodAttributes.Private | MethodAttributes.HideBySig;
        internal static Type[] EmptyTypeArray = new Type[] { };
        internal static string[] EmptyStringArray = new string[] { };

        TypeBuilder typeBuilder;
        MethodBuilder methodBuilder;
        ILGenerator ilGen;
        Dictionary<string, ArgBuilder> argList;
        LocalScope currentScope;
        // Stores a queue of free locals available in the context of the method, keyed by
        // type and name of the local
        Dictionary<Tuple<Type, string>, Queue<LocalBuilder>> freeLocals;
        Stack blockStack;
        Label methodEndLabel;

        internal CodeGenerator(TypeBuilder typeBuilder) {
            System.Diagnostics.Debug.Assert(typeBuilder != null);
            this.typeBuilder = typeBuilder;
        }

        internal static bool IsNullableGenericType(Type type) {
            return type.Name == "Nullable`1";
        }

        internal static void AssertHasInterface(Type type, Type iType) {
#if DEBUG && disabled
            Debug.Assert(iType.IsInterface);
            foreach (Type iFace in type.GetInterfaces()) {
                if (iFace == iType)
                    return;
            }
            Debug.Assert(false);
#endif
        }

        internal void BeginMethod(Type returnType, string methodName, Type[] argTypes, string[] argNames, MethodAttributes methodAttributes) {
            this.methodBuilder = typeBuilder.DefineMethod(methodName, methodAttributes, returnType, argTypes);
            this.ilGen = methodBuilder.GetILGenerator();
            InitILGeneration(argTypes, argNames, (methodBuilder.Attributes & MethodAttributes.Static) == MethodAttributes.Static);
        }

        internal void BeginMethod(Type returnType, MethodBuilderInfo methodBuilderInfo, Type[] argTypes, string[] argNames, MethodAttributes methodAttributes) {
#if DEBUG
            methodBuilderInfo.Validate(returnType, argTypes, methodAttributes);
#endif
            this.methodBuilder = methodBuilderInfo.MethodBuilder;
            this.ilGen = methodBuilder.GetILGenerator();
            InitILGeneration(argTypes, argNames, (methodBuilder.Attributes & MethodAttributes.Static) == MethodAttributes.Static);
        }

        void InitILGeneration(Type[] argTypes, string[] argNames, bool isStatic) {
            this.methodEndLabel = ilGen.DefineLabel();
            this.retLabel = ilGen.DefineLabel();
            this.blockStack = new Stack();
            this.whileStack = new Stack();
            this.currentScope = new LocalScope();
            this.freeLocals = new Dictionary<Tuple<Type, string>, Queue<LocalBuilder>>(); 
            this.argList = new Dictionary<string, ArgBuilder>();
            // this ptr is arg 0 for non static, assuming ref type (not value type) 
            if (!isStatic)
                argList.Add("this", new ArgBuilder("this", 0, this.typeBuilder.BaseType));
            for (int i = 0; i < argTypes.Length; i++) {
                ArgBuilder arg = new ArgBuilder(argNames[i], argList.Count, argTypes[i]);
                argList.Add(arg.Name, arg);
                this.methodBuilder.DefineParameter(arg.Index, ParameterAttributes.None, arg.Name);
            }
        }

        internal MethodBuilder EndMethod() {
            MarkLabel(methodEndLabel);
            Ret();

            MethodBuilder retVal = null;
            retVal = methodBuilder;
            methodBuilder = null;
            ilGen = null;
            freeLocals = null;
            blockStack = null;
            whileStack = null;
            argList = null;
            currentScope = null;
            retLocal = null;
            return retVal;
        }

        internal MethodBuilder MethodBuilder {
            get { return this.methodBuilder; }
        }

        internal static Exception NotSupported(string msg) {
            System.Diagnostics.Debug.Assert(false, msg);
            return new NotSupportedException(msg);
        }

        internal ArgBuilder GetArg(string name) {
            System.Diagnostics.Debug.Assert(argList != null && argList.ContainsKey(name));
            return (ArgBuilder)argList[name];
        }

        internal LocalBuilder GetLocal(string name) {
            System.Diagnostics.Debug.Assert(currentScope != null && currentScope.ContainsKey(name));
            return currentScope[name];
        }

        internal LocalBuilder retLocal;
        internal Label retLabel;
        internal LocalBuilder ReturnLocal {
            get {
                if (retLocal == null)
                    retLocal = DeclareLocal(this.methodBuilder.ReturnType, "_ret");
                return retLocal;
            }
        }
        internal Label ReturnLabel {
            get { return retLabel; }
        }

        Dictionary<Type, LocalBuilder> TmpLocals = new Dictionary<Type, LocalBuilder>();
        internal LocalBuilder GetTempLocal(Type type) {
            LocalBuilder localTmp;
            if (!TmpLocals.TryGetValue(type, out localTmp)) {
                localTmp = DeclareLocal(type, "_tmp" + TmpLocals.Count);
                TmpLocals.Add(type, localTmp);
            }
            return localTmp;
        }

        internal Type GetVariableType(object var) {
            if (var is ArgBuilder)
                return ((ArgBuilder)var).ArgType;
            else if (var is LocalBuilder)
                return ((LocalBuilder)var).LocalType;
            else
                return var.GetType();
        }

        internal object GetVariable(string name) {
            object var;
            if (TryGetVariable(name, out var))
                return var;
            System.Diagnostics.Debug.Assert(false);
            return null;
        }

        internal bool TryGetVariable(string name, out object variable) {
            LocalBuilder loc;
            if (currentScope != null && currentScope.TryGetValue(name, out loc)) {
                variable = loc;
                return true;
            }
            ArgBuilder arg;
            if (argList != null && argList.TryGetValue(name, out arg)) {
                variable = arg;
                return true;
            }
            int val;
            if (Int32.TryParse(name, out val)) {
                variable = val;
                return true;
            }
            variable = null;
            return false;
        }

#if NotUsed
        internal LocalBuilder DeclareLocal(Type type, string name, object initialValue) {
            LocalBuilder local = DeclareLocal(type, name);
            Load(initialValue);
            Store(local);
            return local;
        }
#endif
        internal void EnterScope() {
            LocalScope newScope = new LocalScope(this.currentScope);
            this.currentScope = newScope;
        }

        internal void ExitScope()
        {
            Debug.Assert(this.currentScope.parent != null);
            this.currentScope.AddToFreeLocals(freeLocals);
            this.currentScope = this.currentScope.parent;
        }

        private bool TryDequeueLocal(Type type, string name, out LocalBuilder local) {
            // This method can only be called between BeginMethod and EndMethod (i.e.
            // while we are emitting code for a method
            Debug.Assert(freeLocals != null);

            Queue<LocalBuilder> freeLocalQueue;
            Tuple<Type, string> key = new Tuple<Type, string>(type, name);
            if (freeLocals.TryGetValue(key, out freeLocalQueue)) {
                local = freeLocalQueue.Dequeue();

                // If the queue is empty, remove this key from the dictionary
                // of free locals
                if (freeLocalQueue.Count == 0) {
                    freeLocals.Remove(key);
                }
                return true;
            }
            local = null;
            return false;
        }

        internal LocalBuilder DeclareLocal(Type type, string name) {
            Debug.Assert(!currentScope.ContainsKey(name));
            LocalBuilder local;
            if (!TryDequeueLocal(type, name, out local)) {
                local = ilGen.DeclareLocal(type, false);
#if disabled
                if (DiagnosticsSwitches.KeepTempFiles.Enabled)
                    local.SetLocalSymInfo(name);
#endif
            }
            currentScope[name] = local;
            return local;
        }

        internal LocalBuilder DeclareOrGetLocal(Type type, string name) {
            LocalBuilder local;
            if (!currentScope.TryGetValue(name, out local))
                local = DeclareLocal(type, name);
            else
                Debug.Assert(local.LocalType == type);
            return local;
        }

#if NotUsed
        Dictionary<string, int> parameterMapping = new Dictionary<string, int>();
        internal Dictionary<string, int> ParameterMapping { get { return this.parameterMapping; } }
        internal ParameterBuilder DefineParameter(int index, ParameterAttributes attributes, string name)
        {
            if (this.parameterMapping == null)
            {
                this.parameterMapping = new Dictionary<string, int>();
            }
            this.parameterMapping.Add(name, index);
            return this.methodBuilder.DefineParameter(index, attributes, name);
        }

        internal void Set(LocalBuilder local, object value)
        {
            Load(value);
            Store(local);
        }
#endif
        internal object For(LocalBuilder local, object start, object end) {
            ForState forState = new ForState(local, DefineLabel(), DefineLabel(), end);
            if (forState.Index != null) {
                Load(start);
                Stloc(forState.Index);
                Br(forState.TestLabel);
            }
            MarkLabel(forState.BeginLabel);
            blockStack.Push(forState);
            return forState;
        }

        internal void EndFor() {
            object stackTop = blockStack.Pop();
            ForState forState = stackTop as ForState;
            Debug.Assert(forState != null);
            if (forState.Index != null) {
                Ldloc(forState.Index);
                Ldc(1);
                Add();
                Stloc(forState.Index);
                MarkLabel(forState.TestLabel);
                Ldloc(forState.Index);
                Load(forState.End);
                Type varType = GetVariableType(forState.End);
                if (varType.IsArray) {
                    Ldlen();
                }
                else {
#if DEBUG
                    CodeGenerator.AssertHasInterface(varType, typeof(ICollection));
#endif
                    MethodInfo ICollection_get_Count = typeof(ICollection).GetMethod(
                          "get_Count",
                          CodeGenerator.InstanceBindingFlags,
                          null,
                          CodeGenerator.EmptyTypeArray,
                          null
                          );
                    Call(ICollection_get_Count);
                }
                Blt(forState.BeginLabel);
            }
            else
                Br(forState.BeginLabel);
        }

#if NotUsed
        internal void Break(object forState)
        {
            InternalBreakFor(forState, OpCodes.Br);
        }

        internal void IfTrueBreak(object forState)
        {
            InternalBreakFor(forState, OpCodes.Brtrue);
        }

        internal void IfFalseBreak(object forState)
        {
            InternalBreakFor(forState, OpCodes.Brfalse);
        }

        internal void InternalBreakFor(object userForState, OpCode branchInstruction)
        {
            foreach (object block in blockStack)
            {
                ForState forState = block as ForState;
                if (forState != null && (object)forState == userForState)
                {
                    if (!forState.RequiresEndLabel)
                    {
                        forState.EndLabel = DefineLabel();
                        forState.RequiresEndLabel = true;
                    }
                    ilGen.Emit(branchInstruction, forState.EndLabel);
                    break;
                }
            }
        }

        internal void ForEach(LocalBuilder local, Type elementType, Type enumeratorType,
            LocalBuilder enumerator, MethodInfo getCurrentMethod)
        {
            ForState forState = new ForState(local, DefineLabel(), DefineLabel(), enumerator);

            Br(forState.TestLabel);
            MarkLabel(forState.BeginLabel);

            Call(enumerator, getCurrentMethod);

            ConvertValue(elementType, GetVariableType(local));
            Stloc(local);
            blockStack.Push(forState);
        }

        internal void EndForEach(MethodInfo moveNextMethod)
        {
            object stackTop = blockStack.Pop();
            ForState forState = stackTop as ForState;
            if (forState == null)
                ThrowMismatchException(stackTop);

            MarkLabel(forState.TestLabel);

            object enumerator = forState.End;
            Call(enumerator, moveNextMethod);


            Brtrue(forState.BeginLabel);
            if (forState.RequiresEndLabel)
                MarkLabel(forState.EndLabel);
        }

        internal void IfNotDefaultValue(object value)
        {
            Type type = GetVariableType(value);
            TypeCode typeCode = Type.GetTypeCode(type);
            if ((typeCode == TypeCode.Object && type.IsValueType) ||
                typeCode == TypeCode.DateTime || typeCode == TypeCode.Decimal)
            {
                LoadDefaultValue(type);
                ConvertValue(type, Globals.TypeOfObject);
                Load(value);
                ConvertValue(type, Globals.TypeOfObject);
                Call(ObjectEquals);
                IfNot();
            }
            else
            {
                LoadDefaultValue(type);
                Load(value);
                If(Cmp.NotEqualTo);
            }
        }
#endif

        internal void If() {
            InternalIf(false);
        }

        internal void IfNot() {
            InternalIf(true);
        }

        static OpCode[] BranchCodes = new OpCode[] {
            OpCodes.Bge,
            OpCodes.Bne_Un,
            OpCodes.Bgt,
            OpCodes.Ble,
            OpCodes.Beq,
            OpCodes.Blt,
        };

        OpCode GetBranchCode(Cmp cmp) {
            return BranchCodes[(int)cmp];
        }

#if NotUsed
        Cmp GetCmpInverse(Cmp cmp)
        {
            switch (cmp) {
                case Cmp.LessThan:
                    return Cmp.GreaterThanOrEqualTo;
                case Cmp.EqualTo:
                    return Cmp.NotEqualTo;
                case Cmp.LessThanOrEqualTo:
                    return Cmp.GreaterThan;
                case Cmp.GreaterThan:
                    return Cmp.LessThanOrEqualTo;
                case Cmp.NotEqualTo:
                    return Cmp.EqualTo;
                default:
                    Debug.Assert(cmp == Cmp.GreaterThanOrEqualTo, "Unexpected cmp");
                    return Cmp.LessThan;
            }
        }
#endif

        internal void If(Cmp cmpOp) {
            IfState ifState = new IfState();
            ifState.EndIf = DefineLabel();
            ifState.ElseBegin = DefineLabel();
            ilGen.Emit(GetBranchCode(cmpOp), ifState.ElseBegin);
            blockStack.Push(ifState);
        }

        internal void If(object value1, Cmp cmpOp, object value2) {
            Load(value1);
            Load(value2);
            If(cmpOp);
        }

        internal void Else() {
            IfState ifState = PopIfState();
            Br(ifState.EndIf);
            MarkLabel(ifState.ElseBegin);

            ifState.ElseBegin = ifState.EndIf;
            blockStack.Push(ifState);
        }

#if NotUsed
        internal void ElseIf(object value1, Cmp cmpOp, object value2)
        {
            IfState ifState = (IfState)blockStack.Pop();
            Br(ifState.EndIf);
            MarkLabel(ifState.ElseBegin);

            Load(value1);
            Load(value2);
            ifState.ElseBegin = DefineLabel();
            ilGen.Emit(GetBranchCode(cmpOp), ifState.ElseBegin);
            blockStack.Push(ifState);
        }
#endif

        internal void EndIf() {
            IfState ifState = PopIfState();
            if (!ifState.ElseBegin.Equals(ifState.EndIf))
                MarkLabel(ifState.ElseBegin);
            MarkLabel(ifState.EndIf);
        }

        Stack leaveLabels = new Stack();
        internal void BeginExceptionBlock() {
            leaveLabels.Push(DefineLabel());
            ilGen.BeginExceptionBlock();
        }

        internal void BeginCatchBlock(Type exception) {
            ilGen.BeginCatchBlock(exception);
        }

        internal void EndExceptionBlock() {
            ilGen.EndExceptionBlock();
            ilGen.MarkLabel((Label)leaveLabels.Pop());
        }

        internal void Leave() {
            ilGen.Emit(OpCodes.Leave, (Label)leaveLabels.Peek());
        }

#if NotUsed
        internal void VerifyParameterCount(MethodInfo methodInfo, int expectedCount)
        {
            if (methodInfo.GetParameters().Length != expectedCount)
                throw Utility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.ParameterCountMismatch, methodInfo.Name, methodInfo.GetParameters().Length, expectedCount)));
        }

        internal void Call(object thisObj, MethodInfo methodInfo)
        {
            VerifyParameterCount(methodInfo, 0);
            LoadThis(thisObj, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1)
        {
            VerifyParameterCount(methodInfo, 1);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1, object param2)
        {
            VerifyParameterCount(methodInfo, 2);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            LoadParam(param2, 2, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3)
        {
            VerifyParameterCount(methodInfo, 3);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            LoadParam(param2, 2, methodInfo);
            LoadParam(param3, 3, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4)
        {
            VerifyParameterCount(methodInfo, 4);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            LoadParam(param2, 2, methodInfo);
            LoadParam(param3, 3, methodInfo);
            LoadParam(param4, 4, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4, object param5)
        {
            VerifyParameterCount(methodInfo, 5);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            LoadParam(param2, 2, methodInfo);
            LoadParam(param3, 3, methodInfo);
            LoadParam(param4, 4, methodInfo);
            LoadParam(param5, 5, methodInfo);
            Call(methodInfo);
        }

        internal void Call(object thisObj, MethodInfo methodInfo, object param1, object param2, object param3, object param4, object param5, object param6)
        {
            VerifyParameterCount(methodInfo, 6);
            LoadThis(thisObj, methodInfo);
            LoadParam(param1, 1, methodInfo);
            LoadParam(param2, 2, methodInfo);
            LoadParam(param3, 3, methodInfo);
            LoadParam(param4, 4, methodInfo);
            LoadParam(param5, 5, methodInfo);
            LoadParam(param6, 6, methodInfo);
            Call(methodInfo);
        }
#endif

        internal void Call(MethodInfo methodInfo) {
            Debug.Assert(methodInfo != null);
            if (methodInfo.IsVirtual && !methodInfo.DeclaringType.GetTypeInfo().IsValueType)
                ilGen.Emit(OpCodes.Callvirt, methodInfo);
            else
                ilGen.Emit(OpCodes.Call, methodInfo);
        }

        internal void Call(ConstructorInfo ctor) {
            Debug.Assert(ctor != null);
            ilGen.Emit(OpCodes.Call, ctor);
        }

        internal void New(ConstructorInfo constructorInfo) {
            Debug.Assert(constructorInfo != null);
            ilGen.Emit(OpCodes.Newobj, constructorInfo);
        }

#if NotUsed
        internal void New(ConstructorInfo constructorInfo, object param1)
        {
            LoadParam(param1, 1, constructorInfo);
            New(constructorInfo);
        }
#endif

        internal void InitObj(Type valueType) {
            ilGen.Emit(OpCodes.Initobj, valueType);
        }

        internal void NewArray(Type elementType, object len) {
            Load(len);
            ilGen.Emit(OpCodes.Newarr, elementType);
        }

#if NotUsed
        internal void IgnoreReturnValue()
        {
            Pop();
        }
#endif

        internal void LoadArrayElement(object obj, object arrayIndex) {
            Type objType = GetVariableType(obj).GetElementType();
            Load(obj);
            Load(arrayIndex);
            if (IsStruct(objType)) {
                Ldelema(objType);
                Ldobj(objType);
            }
            else
                Ldelem(objType);
        }

        internal void StoreArrayElement(object obj, object arrayIndex, object value) {
            Type arrayType = GetVariableType(obj);
            if (arrayType == typeof(Array)) {
                Load(obj);
                Call(typeof(Array).GetMethod("SetValue", new Type[] { typeof(object), typeof(int) }));
            }
            else {
                Type objType = arrayType.GetElementType();
                Load(obj);
                Load(arrayIndex);
                if (IsStruct(objType))
                    Ldelema(objType);
                Load(value);
                ConvertValue(GetVariableType(value), objType);
                if (IsStruct(objType))
                    Stobj(objType);
                else
                    Stelem(objType);
            }
        }

        static bool IsStruct(Type objType) {
            TypeInfo info = objType.GetTypeInfo();
            return info.IsValueType && !info.IsPrimitive;
        }

        internal Type LoadMember(object obj, MemberInfo memberInfo) {
            if (GetVariableType(obj).GetTypeInfo().IsValueType)
                LoadAddress(obj);
            else
                Load(obj);
            return LoadMember(memberInfo);
        }

        private static MethodInfo GetPropertyMethodFromBaseType(PropertyInfo propertyInfo, bool isGetter) {
            // we only invoke this when the propertyInfo does not have a GET or SET method on it
           
            Type currentType = propertyInfo.DeclaringType.GetTypeInfo().BaseType;
            PropertyInfo currentProperty;
            string propertyName = propertyInfo.Name;
            MethodInfo result = null;

            while (currentType != null) {
                currentProperty = currentType.GetProperty(propertyName);

                if (currentProperty != null) {
                    if (isGetter) {
                        result = currentProperty.GetGetMethod(true);
                    }
                    else {
                        result = currentProperty.GetSetMethod(true);
                    }

                    if (result != null) {
                        // we found the GetMethod/SetMethod on the type closest to the current declaring type
                        break;
                    }
                }

                // keep looking at the base type like compiler does
                currentType = currentType.GetTypeInfo().BaseType;
            }

            return result;
        }

        internal Type LoadMember(MemberInfo memberInfo) {
            Type memberType = null;
            FieldInfo fieldInfo = memberInfo as FieldInfo;

            if (fieldInfo != null) {
                memberType = fieldInfo.FieldType;
                if (fieldInfo.IsStatic) {
                    ilGen.Emit(OpCodes.Ldsfld, fieldInfo);
                }
                else {
                    ilGen.Emit(OpCodes.Ldfld, fieldInfo);
                }
            }
            else {
                PropertyInfo property = (PropertyInfo)memberInfo;
                memberType = property.PropertyType;
                if (property != null) {
                    MethodInfo getMethod = property.GetGetMethod(true);

                    if (getMethod == null)
                    {
                        getMethod = GetPropertyMethodFromBaseType(property, true);
                    }

                    System.Diagnostics.Debug.Assert(getMethod != null);
                    Call(getMethod);
                }
            }

            return memberType;
        }

        internal Type LoadMemberAddress(MemberInfo memberInfo) {
            Type memberType = null;
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null) {
                memberType = fieldInfo.FieldType;
                if (fieldInfo.IsStatic) {
                    ilGen.Emit(OpCodes.Ldsflda, fieldInfo);
                }
                else {
                    ilGen.Emit(OpCodes.Ldflda, fieldInfo);
                }
            }
            else {
                PropertyInfo property = (PropertyInfo)memberInfo;
                memberType = property.PropertyType;
                if (property != null) {
                    MethodInfo getMethod = property.GetGetMethod(true);

                    if (getMethod == null)
                    {
                        getMethod = GetPropertyMethodFromBaseType(property, true);
                    }

                    System.Diagnostics.Debug.Assert(getMethod != null);
                    Call(getMethod);

                    LocalBuilder tmpLoc = GetTempLocal(memberType);
                    Stloc(tmpLoc);
                    Ldloca(tmpLoc);
                }
            }

            return memberType;
        }

        internal void StoreMember(MemberInfo memberInfo) {
            FieldInfo fieldInfo = memberInfo as FieldInfo;
            if (fieldInfo != null) {
                if (fieldInfo.IsStatic) {
                    ilGen.Emit(OpCodes.Stsfld, fieldInfo);
                }
                else {
                    ilGen.Emit(OpCodes.Stfld, fieldInfo);
                }
            }
            else {
                PropertyInfo property = (PropertyInfo)memberInfo;
                if (property != null) {
                    MethodInfo setMethod = property.GetSetMethod(true);

                    if (setMethod == null)
                    {
                        setMethod = GetPropertyMethodFromBaseType(property, false);
                    }

                    System.Diagnostics.Debug.Assert(setMethod != null);
                    Call(setMethod);
                }
            }
        }

#if NotUsed
        internal void LoadDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                switch (Type.GetTypeCode(type))
                {
                case TypeCode.Boolean:
                    Ldc(false);
                    break;
                case TypeCode.Char:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                    Ldc(0);
                    break;
                case TypeCode.Int64:
                case TypeCode.UInt64:
                    Ldc(0L);
                    break;
                case TypeCode.Single:
                    Ldc(0.0F);
                    break;
                case TypeCode.Double:
                    Ldc(0.0);
                    break;
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                default:
                    LocalBuilder zero = DeclareLocal(type, "zero");
                    LoadAddress(zero);
                    InitObj(type);
                    Load(zero);
                    break;
                }
            }
            else
                Load(null);
        }
#endif

        internal void Load(object obj) {
            if (obj == null)
                ilGen.Emit(OpCodes.Ldnull);
            else if (obj is ArgBuilder)
                Ldarg((ArgBuilder)obj);
            else if (obj is LocalBuilder)
                Ldloc((LocalBuilder)obj);
            else
                Ldc(obj);
        }

#if NotUsed
        internal void Store(object var)
        {
            if (var is ArgBuilder)
                Starg((ArgBuilder)var);
            else 
            {
                System.Diagnostics.Debug.Assert(var is LocalBuilder);
                Stloc((LocalBuilder)var);
            }
        }

        internal void Dec(object var)
        {
            Load(var);
            Load(1);
            Subtract();
            Store(var);
        }

        internal void Inc(object var)
        {
            Load(var);
            Load(1);
            Add();
            Store(var);
        }
#endif

        internal void LoadAddress(object obj) {
            if (obj is ArgBuilder)
                LdargAddress((ArgBuilder)obj);
            else if (obj is LocalBuilder)
                LdlocAddress((LocalBuilder)obj);
            else
                Load(obj);
        }


        internal void ConvertAddress(Type source, Type target) {
            InternalConvert(source, target, true);
        }

        internal void ConvertValue(Type source, Type target) {
            InternalConvert(source, target, false);
        }

        internal void Castclass(Type target) {
            ilGen.Emit(OpCodes.Castclass, target);
        }

        internal void Box(Type type) {
            ilGen.Emit(OpCodes.Box, type);
        }

        internal void Unbox(Type type) {
            ilGen.Emit(OpCodes.Unbox, type);
        }

        static OpCode[] LdindOpCodes = new OpCode[] {
            OpCodes.Nop,//Empty = 0,
            OpCodes.Nop,//Object = 1,
            OpCodes.Nop,//DBNull = 2,
            OpCodes.Ldind_I1,//Boolean = 3,
            OpCodes.Ldind_I2,//Char = 4,
            OpCodes.Ldind_I1,//SByte = 5,
            OpCodes.Ldind_U1,//Byte = 6,
            OpCodes.Ldind_I2,//Int16 = 7,
            OpCodes.Ldind_U2,//UInt16 = 8,
            OpCodes.Ldind_I4,//Int32 = 9,
            OpCodes.Ldind_U4,//UInt32 = 10,
            OpCodes.Ldind_I8,//Int64 = 11,
            OpCodes.Ldind_I8,//UInt64 = 12,
            OpCodes.Ldind_R4,//Single = 13,
            OpCodes.Ldind_R8,//Double = 14,
            OpCodes.Nop,//Decimal = 15,
            OpCodes.Nop,//DateTime = 16,
            OpCodes.Nop,//17
            OpCodes.Ldind_Ref,//String = 18,
        };


        OpCode GetLdindOpCode(TypeCode typeCode) {
            return LdindOpCodes[(int)typeCode];
            //switch (typeCode)
            //{
            //case TypeCode.Boolean:
            //    return OpCodes.Ldind_I1; // TypeCode.Boolean:
            //case TypeCode.Char:
            //    return OpCodes.Ldind_I2; // TypeCode.Char:
            //case TypeCode.SByte:
            //    return OpCodes.Ldind_I1; // TypeCode.SByte:
            //case TypeCode.Byte:
            //    return OpCodes.Ldind_U1; // TypeCode.Byte:
            //case TypeCode.Int16:
            //    return OpCodes.Ldind_I2; // TypeCode.Int16:
            //case TypeCode.UInt16:
            //    return OpCodes.Ldind_U2; // TypeCode.UInt16:
            //case TypeCode.Int32:
            //    return OpCodes.Ldind_I4; // TypeCode.Int32:
            //case TypeCode.UInt32:
            //    return OpCodes.Ldind_U4; // TypeCode.UInt32:
            //case TypeCode.Int64:
            //    return OpCodes.Ldind_I8; // TypeCode.Int64:
            //case TypeCode.UInt64:
            //    return OpCodes.Ldind_I8; // TypeCode.UInt64:
            //case TypeCode.Single:
            //    return OpCodes.Ldind_R4; // TypeCode.Single:
            //case TypeCode.Double:
            //    return OpCodes.Ldind_R8; // TypeCode.Double:
            //case TypeCode.String:
            //    return OpCodes.Ldind_Ref; // TypeCode.String:
            //default:
            //    return OpCodes.Nop;
            //}
            // REVIEW, stefanph: What's the type code for Ldind_I (natural int)?
        }

        internal void Ldobj(Type type) {
            OpCode opCode = GetLdindOpCode(type.GetTypeCode());
            if (!opCode.Equals(OpCodes.Nop)) {
                ilGen.Emit(opCode);
            }
            else {
                ilGen.Emit(OpCodes.Ldobj, type);
            }
        }

        internal void Stobj(Type type) {
            ilGen.Emit(OpCodes.Stobj, type);
        }

        internal void Ceq() {
            ilGen.Emit(OpCodes.Ceq);
        }

        internal void Clt() {
            ilGen.Emit(OpCodes.Clt);
        }

        internal void Cne() {
            Ceq();
            Ldc(0);
            Ceq();
        }

#if NotUsed
        internal void Bgt(Label label)
        {
            ilGen.Emit(OpCodes.Bgt, label);
        }
#endif

        internal void Ble(Label label) {
            ilGen.Emit(OpCodes.Ble, label);
        }

        internal void Throw() {
            ilGen.Emit(OpCodes.Throw);
        }

        internal void Ldtoken(Type t) {
            ilGen.Emit(OpCodes.Ldtoken, t);
        }

        internal void Ldc(object o) {
            Type valueType = o.GetType();
            if (o is Type) {
                Ldtoken((Type)o);
                Call(typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Static | BindingFlags.Public, null, new Type[] { typeof(RuntimeTypeHandle) }, null));
            }
            else if (valueType.GetTypeInfo().IsEnum) {
                Ldc(((IConvertible)o).ToType(Enum.GetUnderlyingType(valueType), null));
            }
            else {
                switch (valueType.GetTypeCode()) {
                    case TypeCode.Boolean:
                        Ldc((bool)o);
                        break;
                    case TypeCode.Char:
                        Debug.Assert(false, "Char is not a valid schema primitive and should be treated as int in DataContract");
                        throw new NotSupportedException("Char is not a valid schema primitive and should be treated as int in DataContract");
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        Ldc(((IConvertible)o).ToInt32(CultureInfo.InvariantCulture));
                        break;
                    case TypeCode.Int32:
                        Ldc((int)o);
                        break;
                    case TypeCode.UInt32:
                        Ldc((int)(uint)o);
                        break;
                    case TypeCode.UInt64:
                        Ldc((long)(ulong)o);
                        break;
                    case TypeCode.Int64:
                        Ldc((long)o);
                        break;
                    case TypeCode.Single:
                        Ldc((float)o);
                        break;
                    case TypeCode.Double:
                        Ldc((double)o);
                        break;
                    case TypeCode.String:
                        Ldstr((string)o);
                        break;
                    case TypeCode.Decimal:
                        ConstructorInfo Decimal_ctor = typeof(Decimal).GetConstructor(
                             new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Boolean), typeof(Byte) }
                            );
#if disabled
                        ConstructorInfo Decimal_ctor = typeof(Decimal).GetConstructor(
                             CodeGenerator.InstanceBindingFlags,
                             null,
                             new Type[] { typeof(Int32), typeof(Int32), typeof(Int32), typeof(Boolean), typeof(Byte) },
                             null
                             );
#endif
                        int[] bits = Decimal.GetBits((decimal)o);
                        Ldc(bits[0]); // digit
                        Ldc(bits[1]); // digit
                        Ldc(bits[2]); // digit
                        Ldc((bits[3] & 0x80000000) == 0x80000000); // sign
                        Ldc((Byte)((bits[3] >> 16) & 0xFF)); // decimal location
                        New(Decimal_ctor);
                        break;
                    case TypeCode.DateTime:
                        ConstructorInfo DateTime_ctor = typeof(DateTime).GetConstructor(new Type[] { typeof(Int64) });
#if disabled
                        ConstructorInfo DateTime_ctor = typeof(DateTime).GetConstructor(
                            CodeGenerator.InstanceBindingFlags,
                            null,
                            new Type[] { typeof(Int64) },
                            null
                            );
#endif
                        Ldc(((DateTime)o).Ticks); // ticks
                        New(DateTime_ctor);
                        break;
                    case TypeCode.Object:
                    case TypeCode.Empty:
                    //case TypeCode.DBNull:
                    default:
                        Debug.Assert(false, "UnknownConstantType");
                        throw new NotSupportedException("UnknownConstantType"); //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.UnknownConstantType, DataContract.GetClrTypeFullName(valueType))));
                }
            }
        }

        internal void Ldc(bool boolVar) {
            if (boolVar) {
                ilGen.Emit(OpCodes.Ldc_I4_1);
            }
            else {
                ilGen.Emit(OpCodes.Ldc_I4_0);
            }
        }

        internal void Ldc(int intVar) {
            switch (intVar) {
                case -1:
                    ilGen.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    ilGen.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGen.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGen.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGen.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGen.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGen.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    ilGen.Emit(OpCodes.Ldc_I4, intVar);
                    break;
            }
        }

        internal void Ldc(long l) {
            ilGen.Emit(OpCodes.Ldc_I8, l);
        }

        internal void Ldc(float f) {
            ilGen.Emit(OpCodes.Ldc_R4, f);
        }

        internal void Ldc(double d) {
            ilGen.Emit(OpCodes.Ldc_R8, d);
        }

        internal void Ldstr(string strVar) {
            if (strVar == null)
                ilGen.Emit(OpCodes.Ldnull);
            else
                ilGen.Emit(OpCodes.Ldstr, strVar);
        }

        internal void LdlocAddress(LocalBuilder localBuilder) {
            if (localBuilder.LocalType.GetTypeInfo().IsValueType)
                Ldloca(localBuilder);
            else
                Ldloc(localBuilder);
        }

        internal void Ldloc(LocalBuilder localBuilder) {
            ilGen.Emit(OpCodes.Ldloc, localBuilder);
        }

        internal void Ldloc(string name) {
            Debug.Assert(currentScope.ContainsKey(name));
            LocalBuilder local = currentScope[name];
            Ldloc(local);
        }

        internal void Stloc(Type type, string name) {
            LocalBuilder local = null;
            if (!currentScope.TryGetValue(name, out local)) {
                local = DeclareLocal(type, name);
            }
            Debug.Assert(local.LocalType == type);
            Stloc(local);
        }

        internal void Stloc(LocalBuilder local) {
            ilGen.Emit(OpCodes.Stloc, local);
        }

        internal void Ldloc(Type type, string name) {
            Debug.Assert(currentScope.ContainsKey(name));
            LocalBuilder local = currentScope[name];
            Debug.Assert(local.LocalType == type);
            Ldloc(local);
        }

#if NotUsed
        internal void Ldloc(int slot)
        {
            switch (slot)
            {
            case 0:
                ilGen.Emit(OpCodes.Ldloc_0);
                break;
            case 1:
                ilGen.Emit(OpCodes.Ldloc_1);
                break;
            case 2:
                ilGen.Emit(OpCodes.Ldloc_2);
                break;
            case 3:
                ilGen.Emit(OpCodes.Ldloc_3);
                break;
            default:
                if (slot <= 255)
                    ilGen.Emit(OpCodes.Ldloc_S, slot);
                else
                    ilGen.Emit(OpCodes.Ldloc, slot);
                break;
            }
        }

        internal void Stloc(int slot)
        {
            switch (slot)
            {
            case 0:
                ilGen.Emit(OpCodes.Stloc_0);
                break;
            case 1:
                ilGen.Emit(OpCodes.Stloc_1);
                break;
            case 2:
                ilGen.Emit(OpCodes.Stloc_2);
                break;
            case 3:
                ilGen.Emit(OpCodes.Stloc_3);
                break;
            default:
                if (slot <= 255)
                    ilGen.Emit(OpCodes.Stloc_S, slot);
                else
                    ilGen.Emit(OpCodes.Stloc, slot);
                break;
            }
        }

        internal void Ldloca(int slot)
        {
            if (slot <= 255)
                ilGen.Emit(OpCodes.Ldloca_S, slot);
            else
                ilGen.Emit(OpCodes.Ldloca, slot);
        }
#endif

        internal void Ldloca(LocalBuilder localBuilder) {
            ilGen.Emit(OpCodes.Ldloca, localBuilder);
        }

        internal void LdargAddress(ArgBuilder argBuilder) {
            if (argBuilder.ArgType.GetTypeInfo().IsValueType)
                Ldarga(argBuilder);
            else
                Ldarg(argBuilder);
        }

        internal void Ldarg(string arg) {
            Ldarg(GetArg(arg));
        }

        internal void Ldarg(ArgBuilder arg) {
            Ldarg(arg.Index);
        }

        internal void Ldarg(int slot) {
            switch (slot) {
                case 0:
                    ilGen.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGen.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGen.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGen.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (slot <= 255)
                        ilGen.Emit(OpCodes.Ldarg_S, slot);
                    else
                        ilGen.Emit(OpCodes.Ldarg, slot);
                    break;
            }
        }

#if NotUsed
        internal void Starg(ArgBuilder arg)
        {
            Starg(arg.Index);
        }

        internal void Starg(int slot)
        {
            if (slot <= 255)
                ilGen.Emit(OpCodes.Starg_S, slot);
            else
                ilGen.Emit(OpCodes.Starg, slot);
        }
#endif

        internal void Ldarga(ArgBuilder argBuilder) {
            Ldarga(argBuilder.Index);
        }

        internal void Ldarga(int slot) {
            if (slot <= 255)
                ilGen.Emit(OpCodes.Ldarga_S, slot);
            else
                ilGen.Emit(OpCodes.Ldarga, slot);
        }

        internal void Ldlen() {
            ilGen.Emit(OpCodes.Ldlen);
            ilGen.Emit(OpCodes.Conv_I4);
        }

        static OpCode[] LdelemOpCodes = new OpCode[] {
            OpCodes.Nop,//Empty = 0,
            OpCodes.Ldelem_Ref,//Object = 1,
            OpCodes.Ldelem_Ref,//DBNull = 2,
            OpCodes.Ldelem_I1,//Boolean = 3,
            OpCodes.Ldelem_I2,//Char = 4,
            OpCodes.Ldelem_I1,//SByte = 5,
            OpCodes.Ldelem_U1,//Byte = 6,
            OpCodes.Ldelem_I2,//Int16 = 7,
            OpCodes.Ldelem_U2,//UInt16 = 8,
            OpCodes.Ldelem_I4,//Int32 = 9,
            OpCodes.Ldelem_U4,//UInt32 = 10,
            OpCodes.Ldelem_I8,//Int64 = 11,
            OpCodes.Ldelem_I8,//UInt64 = 12,
            OpCodes.Ldelem_R4,//Single = 13,
            OpCodes.Ldelem_R8,//Double = 14,
            OpCodes.Nop,//Decimal = 15,
            OpCodes.Nop,//DateTime = 16,
            OpCodes.Nop,//17
            OpCodes.Ldelem_Ref,//String = 18,
        };

        OpCode GetLdelemOpCode(TypeCode typeCode) {
            return LdelemOpCodes[(int)typeCode];
            //switch (typeCode)
            //{
            //case TypeCode.Object:
            //case TypeCode.DBNull:
            //    return OpCodes.Ldelem_Ref;// TypeCode.Object:
            //case TypeCode.Boolean:
            //    return OpCodes.Ldelem_I1;// TypeCode.Boolean:
            //case TypeCode.Char:
            //    return OpCodes.Ldelem_I2;// TypeCode.Char:
            //case TypeCode.SByte:
            //    return OpCodes.Ldelem_I1;// TypeCode.SByte:
            //case TypeCode.Byte:
            //    return OpCodes.Ldelem_U1;// TypeCode.Byte:
            //case TypeCode.Int16:
            //    return OpCodes.Ldelem_I2;// TypeCode.Int16:
            //case TypeCode.UInt16:
            //    return OpCodes.Ldelem_U2;// TypeCode.UInt16:
            //case TypeCode.Int32:
            //    return OpCodes.Ldelem_I4;// TypeCode.Int32:
            //case TypeCode.UInt32:
            //    return OpCodes.Ldelem_U4;// TypeCode.UInt32:
            //case TypeCode.Int64:
            //    return OpCodes.Ldelem_I8;// TypeCode.Int64:
            //case TypeCode.UInt64:
            //    return OpCodes.Ldelem_I8;// TypeCode.UInt64:
            //case TypeCode.Single:
            //    return OpCodes.Ldelem_R4;// TypeCode.Single:
            //case TypeCode.Double:
            //    return OpCodes.Ldelem_R8;// TypeCode.Double:
            //case TypeCode.String:
            //    return OpCodes.Ldelem_Ref;// TypeCode.String:
            //default:
            //    return OpCodes.Nop;
            //}
        }

        internal void Ldelem(Type arrayElementType) {
            if (arrayElementType.GetTypeInfo().IsEnum) {
                Ldelem(Enum.GetUnderlyingType(arrayElementType));
            }
            else {
                OpCode opCode = GetLdelemOpCode(arrayElementType.GetTypeCode());
                Debug.Assert(!opCode.Equals(OpCodes.Nop));
                if (opCode.Equals(OpCodes.Nop))
                    throw new InvalidOperationException("ArrayTypeIsNotSupported"); //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.ArrayTypeIsNotSupported, DataContract.GetClrTypeFullName(arrayElementType))));
                ilGen.Emit(opCode);
            }
        }
        internal void Ldelema(Type arrayElementType) {
            OpCode opCode = OpCodes.Ldelema;
            ilGen.Emit(opCode, arrayElementType);
        }

        static OpCode[] StelemOpCodes = new OpCode[] {
            OpCodes.Nop,//Empty = 0,
            OpCodes.Stelem_Ref,//Object = 1,
            OpCodes.Stelem_Ref,//DBNull = 2,
            OpCodes.Stelem_I1,//Boolean = 3,
            OpCodes.Stelem_I2,//Char = 4,
            OpCodes.Stelem_I1,//SByte = 5,
            OpCodes.Stelem_I1,//Byte = 6,
            OpCodes.Stelem_I2,//Int16 = 7,
            OpCodes.Stelem_I2,//UInt16 = 8,
            OpCodes.Stelem_I4,//Int32 = 9,
            OpCodes.Stelem_I4,//UInt32 = 10,
            OpCodes.Stelem_I8,//Int64 = 11,
            OpCodes.Stelem_I8,//UInt64 = 12,
            OpCodes.Stelem_R4,//Single = 13,
            OpCodes.Stelem_R8,//Double = 14,
            OpCodes.Nop,//Decimal = 15,
            OpCodes.Nop,//DateTime = 16,
            OpCodes.Nop,//17
            OpCodes.Stelem_Ref,//String = 18,
        };

        OpCode GetStelemOpCode(TypeCode typeCode) {
            return StelemOpCodes[(int)typeCode];
            //switch (typeCode)
            //{
            //case TypeCode.Object:
            //case TypeCode.DBNull:
            //    return OpCodes.Stelem_Ref;// TypeCode.Object:
            //case TypeCode.Boolean:
            //    return OpCodes.Stelem_I1;// TypeCode.Boolean:
            //case TypeCode.Char:
            //    return OpCodes.Stelem_I2;// TypeCode.Char:
            //case TypeCode.SByte:
            //    return OpCodes.Stelem_I1;// TypeCode.SByte:
            //case TypeCode.Byte:
            //    return OpCodes.Stelem_I1;// TypeCode.Byte:
            //case TypeCode.Int16:
            //    return OpCodes.Stelem_I2;// TypeCode.Int16:
            //case TypeCode.UInt16:
            //    return OpCodes.Stelem_I2;// TypeCode.UInt16:
            //case TypeCode.Int32:
            //    return OpCodes.Stelem_I4;// TypeCode.Int32:
            //case TypeCode.UInt32:
            //    return OpCodes.Stelem_I4;// TypeCode.UInt32:
            //case TypeCode.Int64:
            //    return OpCodes.Stelem_I8;// TypeCode.Int64:
            //case TypeCode.UInt64:
            //    return OpCodes.Stelem_I8;// TypeCode.UInt64:
            //case TypeCode.Single:
            //    return OpCodes.Stelem_R4;// TypeCode.Single:
            //case TypeCode.Double:
            //    return OpCodes.Stelem_R8;// TypeCode.Double:
            //case TypeCode.String:
            //    return OpCodes.Stelem_Ref;// TypeCode.String:
            //default:
            //    return OpCodes.Nop;
            //}
        }

        internal void Stelem(Type arrayElementType) {
            if (arrayElementType.GetTypeInfo().IsEnum)
                Stelem(Enum.GetUnderlyingType(arrayElementType));
            else {
                OpCode opCode = GetStelemOpCode(arrayElementType.GetTypeCode());
                if (opCode.Equals(OpCodes.Nop))
                    throw new InvalidOperationException("ArrayTypeIsNotSupported"); //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.ArrayTypeIsNotSupported, DataContract.GetClrTypeFullName(arrayElementType))));
                ilGen.Emit(opCode);
            }
        }

        internal Label DefineLabel() {
            return ilGen.DefineLabel();
        }

        internal void MarkLabel(Label label) {
            ilGen.MarkLabel(label);
        }

        internal void Nop() {
            ilGen.Emit(OpCodes.Nop);
        }

        internal void Add() {
            ilGen.Emit(OpCodes.Add);
        }

#if NotUsed
        internal void Subtract()
        {
            ilGen.Emit(OpCodes.Sub);
        }

        internal void And()
        {
            ilGen.Emit(OpCodes.And);
        }
        internal void Or()
        {
            ilGen.Emit(OpCodes.Or);
        }

        internal void Not()
        {
            ilGen.Emit(OpCodes.Not);
        }
#endif

        internal void Ret() {
            ilGen.Emit(OpCodes.Ret);
        }

        internal void Br(Label label) {
            ilGen.Emit(OpCodes.Br, label);
        }

        internal void Br_S(Label label) {
            ilGen.Emit(OpCodes.Br_S, label);
        }

        internal void Blt(Label label) {
            ilGen.Emit(OpCodes.Blt, label);
        }

        internal void Brfalse(Label label) {
            ilGen.Emit(OpCodes.Brfalse, label);
        }

        internal void Brtrue(Label label) {
            ilGen.Emit(OpCodes.Brtrue, label);
        }

        internal void Pop() {
            ilGen.Emit(OpCodes.Pop);
        }

        internal void Dup() {
            ilGen.Emit(OpCodes.Dup);
        }

#if !SILVERLIGHT // Not in SL
        internal void Ldftn(MethodInfo methodInfo) {
            ilGen.Emit(OpCodes.Ldftn, methodInfo);
        }
#endif

#if NotUsed
        void LoadThis(object thisObj, MethodInfo methodInfo)
        {
            if (thisObj != null && !methodInfo.IsStatic)
            {
                LoadAddress(thisObj);
                ConvertAddress(GetVariableType(thisObj), methodInfo.DeclaringType);
            }
        }

        void LoadParam(object arg, int oneBasedArgIndex, MethodBase methodInfo)
        {
            Load(arg);
            if (arg != null)
                ConvertValue(GetVariableType(arg), methodInfo.GetParameters()[oneBasedArgIndex - 1].ParameterType);
        }
#endif

        void InternalIf(bool negate) {
            IfState ifState = new IfState();
            ifState.EndIf = DefineLabel();
            ifState.ElseBegin = DefineLabel();
            if (negate)
                Brtrue(ifState.ElseBegin);
            else
                Brfalse(ifState.ElseBegin);
            blockStack.Push(ifState);
        }

        static OpCode[] ConvOpCodes = new OpCode[] {
            OpCodes.Nop,//Empty = 0,
            OpCodes.Nop,//Object = 1,
            OpCodes.Nop,//DBNull = 2,
            OpCodes.Conv_I1,//Boolean = 3,
            OpCodes.Conv_I2,//Char = 4,
            OpCodes.Conv_I1,//SByte = 5,
            OpCodes.Conv_U1,//Byte = 6,
            OpCodes.Conv_I2,//Int16 = 7,
            OpCodes.Conv_U2,//UInt16 = 8,
            OpCodes.Conv_I4,//Int32 = 9,
            OpCodes.Conv_U4,//UInt32 = 10,
            OpCodes.Conv_I8,//Int64 = 11,
            OpCodes.Conv_U8,//UInt64 = 12,
            OpCodes.Conv_R4,//Single = 13,
            OpCodes.Conv_R8,//Double = 14,
            OpCodes.Nop,//Decimal = 15,
            OpCodes.Nop,//DateTime = 16,
            OpCodes.Nop,//17
            OpCodes.Nop,//String = 18,
        };

        OpCode GetConvOpCode(TypeCode typeCode) {
            return ConvOpCodes[(int)typeCode];
            //switch (typeCode)
            //{
            //case TypeCode.Boolean:
            //    return OpCodes.Conv_I1;// TypeCode.Boolean:
            //case TypeCode.Char:
            //    return OpCodes.Conv_I2;// TypeCode.Char:
            //case TypeCode.SByte:
            //    return OpCodes.Conv_I1;// TypeCode.SByte:
            //case TypeCode.Byte:
            //    return OpCodes.Conv_U1;// TypeCode.Byte:
            //case TypeCode.Int16:
            //    return OpCodes.Conv_I2;// TypeCode.Int16:
            //case TypeCode.UInt16:
            //    return OpCodes.Conv_U2;// TypeCode.UInt16:
            //case TypeCode.Int32:
            //    return OpCodes.Conv_I4;// TypeCode.Int32:
            //case TypeCode.UInt32:
            //    return OpCodes.Conv_U4;// TypeCode.UInt32:
            //case TypeCode.Int64:
            //    return OpCodes.Conv_I8;// TypeCode.Int64:
            //case TypeCode.UInt64:
            //    return OpCodes.Conv_U8;// TypeCode.UInt64:
            //case TypeCode.Single:
            //    return OpCodes.Conv_R4;// TypeCode.Single:
            //case TypeCode.Double:
            //    return OpCodes.Conv_R8;// TypeCode.Double:
            //default:
            //    return OpCodes.Nop;
            //}
        }

        void InternalConvert(Type source, Type target, bool isAddress) {
            if (target == source)
                return;
            TypeInfo targetInfo = target.GetTypeInfo();
            TypeInfo sourceInfo = source.GetTypeInfo();
            if (targetInfo.IsValueType) {
                if (sourceInfo.IsValueType) {
                    OpCode opCode = GetConvOpCode(target.GetTypeCode());
                    if (opCode.Equals(OpCodes.Nop)) {
                        //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.NoConversionPossibleTo, DataContract.GetClrTypeFullName(target))));
                        throw new CodeGeneratorConversionException(source, target, isAddress,"NoConversionPossibleTo");
                    }
                    else {
                        ilGen.Emit(opCode);
                    }
                } 
                else if (source.IsAssignableFrom(target)) {
                    Unbox(target);
                    if (!isAddress)
                        Ldobj(target);
                }
                else {
                    //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.IsNotAssignableFrom, DataContract.GetClrTypeFullName(target), DataContract.GetClrTypeFullName(source))));
                    throw new CodeGeneratorConversionException(source, target, isAddress, "IsNotAssignableFrom");
                }
            }
            else if (target.IsAssignableFrom(source)) {
                if (sourceInfo.IsValueType) {
                    if (isAddress)
                        Ldobj(source);
                    Box(source);
                }
            }
            else if (source.IsAssignableFrom(target)) {
                //assert(source.IsValueType == false);
                Castclass(target);
            }
            else if (targetInfo.IsInterface || sourceInfo.IsInterface) {
                Castclass(target);
            }
            else {
                //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.IsNotAssignableFrom, DataContract.GetClrTypeFullName(target), DataContract.GetClrTypeFullName(source))));
                throw new CodeGeneratorConversionException(source, target, isAddress, "IsNotAssignableFrom");
            }
        }

        IfState PopIfState() {
            object stackTop = blockStack.Pop();
            IfState ifState = stackTop as IfState;
            Debug.Assert(ifState != null);
            return ifState;
        }

#if disabled
        static internal AssemblyBuilder CreateAssemblyBuilder(AppDomain appDomain, string name) {
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = name;
            assemblyName.Version = new Version(1, 0, 0, 0);
            if (DiagnosticsSwitches.KeepTempFiles.Enabled)
                return appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, TempFilesLocation);
            else
                return appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        }
        static string tempFilesLocation = null;
        internal static string TempFilesLocation {
            get {
                if (tempFilesLocation == null) {
                    // Return different XmlSerializerSection from legacy assembly due to its register config handlers
                    object section = ConfigurationManager.GetSection(ConfigurationStrings.XmlSerializerSectionPath);
                    string location = null;
                    if (section != null) {
                        XmlSerializerSection configSection = section as XmlSerializerSection;
                        if (configSection != null) {
                            location = configSection.TempFilesLocation;
                        }
                    }
                    if (location != null) {
                        tempFilesLocation = location.Trim();
                    }
                    else {
                        tempFilesLocation = Path.GetTempPath();
                    }
                }
                return tempFilesLocation;
            }
            set {
                tempFilesLocation = value;
            }
        }
#endif

        static internal ModuleBuilder CreateModuleBuilder(AssemblyBuilder assemblyBuilder, string name) {
#if disabled
            if (DiagnosticsSwitches.KeepTempFiles.Enabled)
                return assemblyBuilder.DefineDynamicModule(name, name + ".dll", true);
            else
#endif
                return assemblyBuilder.DefineDynamicModule(name);
        }
        static internal TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, string name, TypeAttributes attributes, Type parent, Type[] interfaces) {
            // parent is nullable if no base class
            return moduleBuilder.DefineType(TempAssembly.GeneratedAssemblyNamespace + "." + name,
                attributes, parent, interfaces);
        }

#if NotUsed
        internal void EmitSourceInstruction(string line)
        {
            EmitSourceLine("    " + line);
        }

        internal void EmitSourceLabel(string line)
        {
            EmitSourceLine(line);
        }

        internal void EmitSourceComment(string comment)
        {
            EmitSourceInstruction("// " + comment);
        }

        internal void EmitSourceLine(string line)
        {
            if (codeGenTrace != CodeGenTrace.None)
                Console.WriteLine(String.Format("{0:X4}: {1}", lineNo++, line));
            SerializationTrace.WriteInstruction(lineNo++, line);
            if (ilGen != null && codeGenTrace == CodeGenTrace.Tron)
            {
                ilGen.Emit(OpCodes.Ldstr, string.Format(CultureInfo.InvariantCulture, "{0:00000}: {1}", lineNo-1, line));
                ilGen.Emit(OpCodes.Call, XmlFormatGeneratorStatics.TraceInstructionMethod);
            }
        }

        internal void EmitStackTop(Type stackTopType)
        {
            if (codeGenTrace != CodeGenTrace.Tron)
                return;
            codeGenTrace = CodeGenTrace.None;
            Dup();
            ToString(stackTopType);
            LocalBuilder topValue = DeclareLocal(Globals.TypeOfString, "topValue");
            Store(topValue);
            Load("//value = ");
            Load(topValue);
            Concat2();
            Call(XmlFormatGeneratorStatics.TraceInstructionMethod);
            codeGenTrace = CodeGenTrace.Tron;
        }

        internal void ToString(Type type)
        {
            if (type.IsValueType)
            {
                Box(type);
                Call(ObjectToString);
            }
            else
            {
                Dup();
                Load(null);
                If(Cmp.EqualTo);
                Pop();
                Load("<null>");
                Else();
                if (type.IsArray)
                {
                    LocalBuilder arrayVar = DeclareLocal(type, "arrayVar");
                    Store(arrayVar);
                    Load("{ ");
                    LocalBuilder arrayValueString = DeclareLocal(typeof(string), "arrayValueString");
                    Store(arrayValueString);
                    LocalBuilder i = DeclareLocal(typeof(int), "i");
                    For(i, 0, arrayVar);
                    Load(arrayValueString);
                    LoadArrayElement(arrayVar, i);
                    ToString(arrayVar.LocalType.GetElementType());
                    Load(", ");
                    Concat3();
                    Store(arrayValueString);
                    EndFor();
                    Load(arrayValueString);
                    Load("}");
                    Concat2();
                }
                else
                    Call(ObjectToString);
                EndIf();
            }
        }

        internal void Concat2()
        {
            Call(StringConcat2);
        }

        internal void Concat3()
        {
            Call(StringConcat3);
        }

        internal Label[] Switch(int labelCount)
        {
            SwitchState switchState = new SwitchState(DefineLabel(), DefineLabel());
            Label[] caseLabels = new Label[labelCount];
            for (int i = 0; i < caseLabels.Length; i++)
                caseLabels[i] = DefineLabel();

            if (codeGenTrace != CodeGenTrace.None)
            {
                EmitSourceInstruction("switch (");
                foreach (Label l in caseLabels)
                    EmitSourceInstruction("    " + l.GetHashCode());
                EmitSourceInstruction(") {");
            }
            ilGen.Emit(OpCodes.Switch, caseLabels);
            Br(switchState.DefaultLabel);
            blockStack.Push(switchState);
            return caseLabels;
        }
        internal void Case(Label caseLabel1, string caseLabelName)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("case " + caseLabelName + "{");
            MarkLabel(caseLabel1);
        }

        internal void EndCase()
        {
            object stackTop = blockStack.Peek();
            SwitchState switchState = stackTop as SwitchState;
            if (switchState == null)
                ThrowMismatchException(stackTop);
            Br(switchState.EndOfSwitchLabel);
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("} //end case ");
        }

        internal void DefaultCase()
        {
            object stackTop = blockStack.Peek();
            SwitchState switchState = stackTop as SwitchState;
            if (switchState == null)
                ThrowMismatchException(stackTop);
            MarkLabel(switchState.DefaultLabel);
            switchState.DefaultDefined = true;
        }

        internal void EndSwitch()
        {
            object stackTop = blockStack.Pop();
            SwitchState switchState = stackTop as SwitchState;
            if (switchState == null)
                ThrowMismatchException(stackTop);
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("} //end switch");
            if (!switchState.DefaultDefined)
                MarkLabel(switchState.DefaultLabel);
            MarkLabel(switchState.EndOfSwitchLabel);
        }

        internal void CallStringFormat(string msg, params object[] values)
        {
            NewArray(typeof(object), values.Length);
            if (stringFormatArray == null)
                stringFormatArray = DeclareLocal(typeof(object[]), "stringFormatArray");
            Stloc(stringFormatArray);
            for (int i = 0; i < values.Length; i++)
                StoreArrayElement(stringFormatArray, i, values[i]);

            Load(msg);
            Load(stringFormatArray);
            Call(StringFormat);
        }

        static MethodInfo stringEquals = typeof(string).GetMethod("Equals", new Type[]{ typeof(string), typeof(string)});
        static MethodInfo stringCompare = typeof(string).GetMethod("Compare", new Type[]{ typeof(string), typeof(string)});
        static ConstructorInfo permissionSetCtor = typeof(PermissionSet).GetConstructor(new Type[]{ typeof(PermissionState)});
        static MethodInfo permissionSetDemand = typeof(PermissionSet).GetMethod("Demand", new Type[0]);
#endif

        int initElseIfStack = -1;
        IfState elseIfState;
        internal void InitElseIf() {
            Debug.Assert(initElseIfStack == -1);
            elseIfState = (IfState)blockStack.Pop();
            initElseIfStack = blockStack.Count;
            Br(elseIfState.EndIf);
            MarkLabel(elseIfState.ElseBegin);
        }

        int initIfStack = -1;
        internal void InitIf() {
            Debug.Assert(initIfStack == -1);
            initIfStack = blockStack.Count;
        }

        internal void AndIf(Cmp cmpOp) {
            if (initIfStack == blockStack.Count) {
                initIfStack = -1;
                If(cmpOp);
                return;
            }
            if (initElseIfStack == blockStack.Count) {
                initElseIfStack = -1;
                elseIfState.ElseBegin = DefineLabel();
                ilGen.Emit(GetBranchCode(cmpOp), elseIfState.ElseBegin);
                blockStack.Push(elseIfState);
                return;
            }
            Debug.Assert(initIfStack == -1 && initElseIfStack == -1);
            IfState ifState = (IfState)blockStack.Peek();
            ilGen.Emit(GetBranchCode(cmpOp), ifState.ElseBegin);
        }

        internal void AndIf() {
            if (initIfStack == blockStack.Count) {
                initIfStack = -1;
                If();
                return;
            }
            if (initElseIfStack == blockStack.Count) {
                initElseIfStack = -1;
                elseIfState.ElseBegin = DefineLabel();
                Brfalse(elseIfState.ElseBegin);
                blockStack.Push(elseIfState);
                return;
            }
            Debug.Assert(initIfStack == -1 && initElseIfStack == -1);
            IfState ifState = (IfState)blockStack.Peek();
            Brfalse(ifState.ElseBegin);
        }

#if NotUsed
        internal void ElseIf(object boolVal)
        {
            InternalElseIf(boolVal, false);
        }
        void InternalElseIf(object boolVal, bool negate)
        {
            IfState ifState = (IfState)blockStack.Pop();
            Br(ifState.EndIf);
            MarkLabel(ifState.ElseBegin);

            Load(boolVal);
            ifState.ElseBegin = DefineLabel();
            if (negate)
                Brtrue(ifState.ElseBegin);
            else
                Brfalse(ifState.ElseBegin);
            blockStack.Push(ifState);
        }

        internal void IfString(object s1, Cmp cmpOp, object s2)
        {
            Load(s1);
            Load(s2);
            Call(stringCompare);
            Load(0);
            If(cmpOp);
        }

        internal void ElseIfString(object s1, Cmp cmpOp, object s2)
        {
            IfState ifState = (IfState)blockStack.Pop();
            Br(ifState.EndIf);
            MarkLabel(ifState.ElseBegin);

            Load(s1);
            Load(s2);
            Call(stringCompare);
            Load(0);
            ifState.ElseBegin = DefineLabel();

            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Branch if " + CmpInverse[(int) cmpOp].ToString() + " to " + ifState.ElseBegin.GetHashCode().ToString(NumberFormatInfo.InvariantInfo));

            ilGen.Emit(BranchCode[(int)cmpOp], ifState.ElseBegin);
            blockStack.Push(ifState);
        }

        internal void While(object value1, Cmp cmpOp, object value2)
        {
            IfState ifState = new IfState();
            ifState.EndIf = DefineLabel();
            ifState.ElseBegin = DefineLabel();
            ilGen.MarkLabel(ifState.ElseBegin);
            Load(value1);
            Load(value2);
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Branch if " + CmpInverse[(int) cmpOp].ToString() + " to " + ifState.ElseBegin.GetHashCode().ToString());
            ilGen.Emit(BranchCode[(int)cmpOp], ifState.EndIf);
            blockStack.Push(ifState);
        }

        internal void EndWhile()
        {
            IfState ifState = PopIfState();
            Br(ifState.ElseBegin);
            MarkLabel(ifState.EndIf);
        }

        internal void ElseIfNot(object boolVal)
        {
            InternalElseIf(boolVal, true);
        }

        internal void Ldc(long l)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Ldc.i8 " + l);
            ilGen.Emit(OpCodes.Ldc_I8, l);
        }

        internal void Ldc(float f)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Ldc.r4 " + f);
            ilGen.Emit(OpCodes.Ldc_R4, f);
        }

        internal void Ldc(double d)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Ldc.r8 " + d);
            ilGen.Emit(OpCodes.Ldc_R8, d);
        }
#endif

        internal void IsInst(Type type) {
            ilGen.Emit(OpCodes.Isinst, type);
        }

#if NotUsed
        internal void Clt()
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Clt");
            ilGen.Emit(OpCodes.Clt);
        }

        internal void StringEquals()
        {
            Call(stringEquals);
        }

        internal void StringEquals(object s1, object s2)
        {
            Load(s1);
            ConvertValue(GetVariableType(s1), typeof(string));
            Load(s2);
            ConvertValue(GetVariableType(s2), typeof(string));
            StringEquals();
        }

        internal void Bge(Label label)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceInstruction("Bge " + label.GetHashCode());
            ilGen.Emit(OpCodes.Bge, label);
        }
#endif

        internal void Beq(Label label) {
            ilGen.Emit(OpCodes.Beq, label);
        }

        internal void Bne(Label label) {
            ilGen.Emit(OpCodes.Bne_Un, label);
        }

#if NotUsed
        internal void ResizeArray(object arrayVar, object sizeVar)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("ResizeArray() {");

            Label doResize = DefineLabel();
            Load(arrayVar);
            Load(null);
            Beq(doResize);
            Ldlen(arrayVar);
            Load(sizeVar);
            If(Cmp.NotEqualTo);
            MarkLabel(doResize);
            Type arrayType = GetVariableType(arrayVar);
            Type elementType = arrayType.GetElementType();
            LocalBuilder tempArray = DeclareLocal(arrayType, "tempArray");
            NewArray(elementType, sizeVar);
            Store(tempArray);
            CopyArray(arrayVar, tempArray, sizeVar);
            Load(tempArray);
            Store(arrayVar);
            EndIf();

            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("} // ResizeArray");
        }

        LocalBuilder resizeLen, resizeCounter;
        internal void EnsureArrayCapacity(object arrayVar, object lastElementVar)
        {
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("EnsureArrayCapacity() {");

            Type arrayType = GetVariableType(arrayVar);
            Type elementType = arrayType.GetElementType();
            If(arrayVar, Cmp.EqualTo, null);
            NewArray(elementType, 4);
            Store(arrayVar);
            Else();
            Load(lastElementVar);
            Ldlen(arrayVar);
            If(Cmp.GreaterThanOrEqualTo);
            LocalBuilder tempArray = DeclareLocal(arrayType, "tempArray");
            if (resizeLen == null)
                resizeLen = DeclareLocal(typeof(int), "resizeLen");
            Load(lastElementVar);
            Load(2);
            Mul();
            Store(resizeLen);
            NewArray(elementType, resizeLen);
            Store(tempArray);
            CopyArray(arrayVar, tempArray, arrayVar);
            Load(tempArray);
            Store(arrayVar);
            EndIf();
            EndIf();

            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("} // EnsureArrayCapacity");
        }

        internal void CopyArray(object sourceArray, object destArray, object length)
        {
            If(sourceArray, Cmp.NotEqualTo, null);
            if (resizeCounter == null)
                resizeCounter = DeclareLocal(typeof(int), "resizeCounter");
            For(resizeCounter, 0, length);
            Load(destArray);
            Load(resizeCounter);
            LoadArrayElement(sourceArray, resizeCounter);
            Stelem(GetVariableType(destArray).GetElementType());
            EndFor();
            EndIf();
        }
#endif

        internal void GotoMethodEnd() {
            //limit to only short forward (127 CIL instruction)
            //Br_S(this.methodEndLabel);
            Br(this.methodEndLabel);
        }

        internal class WhileState {
            public Label StartLabel;
            public Label CondLabel;
            public Label EndLabel;
            public WhileState(CodeGenerator ilg) {
                StartLabel = ilg.DefineLabel();
                CondLabel = ilg.DefineLabel();
                EndLabel = ilg.DefineLabel();
            }
        }

        // Usage:
        // WhileBegin()
        //  WhileBreak()
        //  WhileContinue()
        // WhileBeginCondition()
        // (bool on stack)
        // WhileEndCondition()
        // WhileEnd()
        Stack whileStack;
        internal void WhileBegin() {
            WhileState whileState = new WhileState(this);
            Br(whileState.CondLabel);
            MarkLabel(whileState.StartLabel);
            whileStack.Push(whileState);
        }

        internal void WhileEnd() {
            WhileState whileState = (WhileState)whileStack.Pop();
            MarkLabel(whileState.EndLabel);
        }

        internal void WhileBreak() {
            WhileState whileState = (WhileState)whileStack.Peek();
            Br(whileState.EndLabel);
        }

        internal void WhileContinue() {
            WhileState whileState = (WhileState)whileStack.Peek();
            Br(whileState.CondLabel);
        }

        internal void WhileBeginCondition() {
            WhileState whileState = (WhileState)whileStack.Peek();
            // If there are two MarkLabel ILs consecutively, Labels will converge to one label.
            // This could cause the code to look different.  We insert Nop here specifically
            // that the While label stands out.
            Nop();
            MarkLabel(whileState.CondLabel);
        }

        internal void WhileEndCondition() {
            WhileState whileState = (WhileState)whileStack.Peek();
            Brtrue(whileState.StartLabel);
        }

#if NotUsed
        internal void AndWhile(Cmp cmpOp)
        {
            object startWhile = blockStack.Pop();
            AndIf(cmpOp);
            blockStack.Push(startWhile);
        }

        internal void BeginWhileBody()
        {
            Label startWhile = (Label) blockStack.Pop();
            If();
            blockStack.Push(startWhile);
        }

        internal void BeginWhileBody(Cmp cmpOp)
        {
            Label startWhile = (Label) blockStack.Pop();
            If(cmpOp);
            blockStack.Push(startWhile);
        }

        internal Label BeginWhileCondition()
        {
            Label startWhile = DefineLabel();
            MarkLabel(startWhile);
            blockStack.Push(startWhile);
            return startWhile;
        }

        internal void Cne()
        {
            Ceq();
            Load(0);
            Ceq();
        }

        internal void New(ConstructorInfo constructorInfo, object param1, object param2)
        {
            LoadParam(param1, 1, constructorInfo);
            LoadParam(param2, 2, constructorInfo);
            New(constructorInfo);
        }

        //This code is not tested
        internal void Stind(Type type)
        {
            OpCode opCode = StindOpCodes[(int) Type.GetTypeCode(type)];
            if (!opCode.Equals(OpCodes.Nop))
            {
                if (codeGenTrace != CodeGenTrace.None)
                    EmitSourceInstruction(opCode.ToString());
                ilGen.Emit(opCode);
            }
            else
            {
                if (codeGenTrace != CodeGenTrace.None)
                    EmitSourceInstruction("Stobj " + type);
                ilGen.Emit(OpCodes.Stobj, type);
            }
        }
        //This code is not tested
        internal void StoreOutParam(ArgBuilder arg, object value)
        {
            Type destType = arg.ArgType;
            if (!destType.IsByRef)
                throw new InvalidOperationException("OutParametersMustBeByRefTypeReceived"); //.Serialization.DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(SR.GetString(SR.OutParametersMustBeByRefTypeReceived, DataContract.GetClrTypeFullName(destType))));
            destType = destType.GetElementType();

            Type sourceType;
            if (value is ArgBuilder)
                sourceType = ((ArgBuilder) value).ArgType;
            else if (value is LocalBuilder)
                sourceType = ((LocalBuilder) value).LocalType;
            else if (value != null)
                sourceType = value.GetType();
            else
                sourceType = null;

            Load(arg);
            Load(value);
            if (sourceType != null)
                ConvertAddress(sourceType, destType);
            Stind(destType);
        }
        void CheckSecurity(FieldInfo field)
        {
            if (fullTrustDemanded)
                return;
            if (IsProtectedWithSecurity(field))
                DemandFullTrust();
        }

        void CheckSecurity(MethodBase method)
        {
            if (fullTrustDemanded)
                return;
            if (IsProtectedWithSecurity(method))
                DemandFullTrust();
        }

        void CheckSecurity(Type type)
        {
            if (fullTrustDemanded)
                return;
            if (IsProtectedWithSecurity(type))
                DemandFullTrust();
        }

        void CheckSecurity(Assembly assembly)
        {
            if (fullTrustDemanded)
                return;
            if (IsProtectedWithSecurity(assembly))
                DemandFullTrust();
        }

        static bool IsProtectedWithSecurity(FieldInfo field)
        {
            return IsProtectedWithSecurity(field.DeclaringType);
        }

        static bool IsProtectedWithSecurity(Type type)
        {
            return IsProtectedWithSecurity(type.Assembly) || (type.Attributes & TypeAttributes.HasSecurity) != 0;
        }

        static bool IsProtectedWithSecurity(Assembly assembly)
        {
            object[] attrs = assembly.GetCustomAttributes(typeof(AllowPartiallyTrustedCallersAttribute), true);
            bool hasAptca = attrs != null && attrs.Length > 0;
            return !hasAptca;
        }

        void DemandFullTrust()
        {
            fullTrustDemanded = true;
/*
            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("DemandFullTrust() {");

            Ldc(PermissionState.Unrestricted);
            New(permissionSetCtor);
            Call(permissionSetDemand);

            if (codeGenTrace != CodeGenTrace.None)
                EmitSourceComment("}");
*/
        }

        static bool IsProtectedWithSecurity(MethodBase method)
        {
            return false;
            //return (method.Attributes & MethodAttributes.HasSecurity) != 0;
        }

#endif

    }


    internal class ArgBuilder {
        internal string Name;
        internal int Index;
        internal Type ArgType;
        internal ArgBuilder(string name, int index, Type argType) {
            this.Name = name;
            this.Index = index;
            this.ArgType = argType;
        }
    }

    internal class ForState {
        LocalBuilder indexVar;
        Label beginLabel;
        Label testLabel;
        object end;

        internal ForState(LocalBuilder indexVar, Label beginLabel, Label testLabel, object end) {
            this.indexVar = indexVar;
            this.beginLabel = beginLabel;
            this.testLabel = testLabel;
            this.end = end;
        }

        internal LocalBuilder Index {
            get {
                return indexVar;
            }
        }

        internal Label BeginLabel {
            get {
                return beginLabel;
            }
        }

        internal Label TestLabel {
            get {
                return testLabel;
            }
        }

        internal object End {
            get {
                return end;
            }
        }
    }

    internal enum Cmp : int {
        LessThan = 0,
        EqualTo,
        LessThanOrEqualTo,
        GreaterThan,
        NotEqualTo,
        GreaterThanOrEqualTo
    }

    internal class IfState {
        Label elseBegin;
        Label endIf;

        internal Label EndIf {
            get {
                return this.endIf;
            }
            set {
                this.endIf = value;
            }
        }

        internal Label ElseBegin {
            get {
                return this.elseBegin;
            }
            set {
                this.elseBegin = value;
            }
        }

    }

    internal class LocalScope {
        public readonly LocalScope parent;
        readonly Dictionary<string, LocalBuilder> locals;

        // Root scope
        public LocalScope() {
            this.locals = new Dictionary<string, LocalBuilder>();
        }

        public LocalScope(LocalScope parent) : this() {
            this.parent = parent;
        }

        public void Add(string key, LocalBuilder value) {
            locals.Add(key, value);
        }

        public bool ContainsKey(string key) {
            return locals.ContainsKey(key) || (parent != null && parent.ContainsKey(key));
        }

        public bool TryGetValue(string key, out LocalBuilder value) {
            if (locals.TryGetValue(key, out value)) {
                return true;
            }
            else if (parent != null) {
                return parent.TryGetValue(key, out value);
            }
            else {
                value = null;
                return false;
            }
        }

        public LocalBuilder this[string key] {
            get {
                LocalBuilder value;
                TryGetValue(key, out value);
                return value;
            }
            set {
                locals[key] = value;
            }
        }

        public void AddToFreeLocals(Dictionary<Tuple<Type, string>, Queue<LocalBuilder>> freeLocals) {
            foreach (var item in locals) {
                Tuple<Type, string> key = new Tuple<Type, string>(item.Value.LocalType, item.Key);
                Queue<LocalBuilder> freeLocalQueue;
                if (freeLocals.TryGetValue(key, out freeLocalQueue)) {
                    // Add to end of the queue so that it will be re-used in 
                    // FIFO manner
                    freeLocalQueue.Enqueue(item.Value);
                }
                else {
                    freeLocalQueue = new Queue<LocalBuilder>();
                    freeLocalQueue.Enqueue(item.Value);
                    freeLocals.Add(key, freeLocalQueue);
                }
            }
        }
    }

#if NotUsed
    internal class BitFlagsGenerator
    {
        LocalBuilder[] locals;
        CodeGenerator ilg;
        int bitCount;
        internal BitFlagsGenerator(int bitCount, CodeGenerator ilg, string localName)
        {
            this.ilg = ilg;
            this.bitCount = bitCount;
            int localCount = (bitCount+7)/8;
            locals = new LocalBuilder[localCount];
            for (int i=0;i<locals.Length;i++)
                locals[i] = ilg.DeclareLocal(Globals.TypeOfByte, localName+i, (byte)0);
        }
        internal void Store(int bitIndex, bool value)
        {
            LocalBuilder local = locals[GetByteIndex(bitIndex)];
            byte bitValue = GetBitValue(bitIndex);
            if (value)
            {
                ilg.Load(local);
                ilg.Load(bitValue);
                ilg.Or();
                ilg.Stloc(local);
            }
            else
            {
                ilg.Load(local);
                ilg.Load(bitValue);
                ilg.Not();
                ilg.And();
                ilg.Stloc(local);
            }
        }

        internal void Load(int bitIndex)
        {
            LocalBuilder local = locals[GetByteIndex(bitIndex)];
            byte bitValue = GetBitValue(bitIndex);
            ilg.Load(local);
            ilg.Load(bitValue);
            ilg.And();
            ilg.Load(bitValue);
            ilg.Ceq();
        }

        internal void LoadArray()
        {
            LocalBuilder localArray = ilg.DeclareLocal(Globals.TypeOfByteArray, "localArray");
            ilg.NewArray(Globals.TypeOfByte, locals.Length);
            ilg.Store(localArray);
            for (int i=0;i<locals.Length;i++)
                ilg.StoreArrayElement(localArray, i, locals[i]);
            ilg.Load(localArray);
        }

        internal int GetLocalCount()
        {
            return locals.Length;
        }

        internal int GetBitCount()
        {
            return bitCount;
        }

        internal LocalBuilder GetLocal(int i)
        {
            return locals[i];
        }

        internal static bool IsBitSet(byte[] bytes, int bitIndex)
        {
            int byteIndex = GetByteIndex(bitIndex);
            byte bitValue = GetBitValue(bitIndex);
            return (bytes[byteIndex] & bitValue) == bitValue;
        }

        internal static void SetBit(byte[] bytes, int bitIndex)
        {
            int byteIndex = GetByteIndex(bitIndex);
            byte bitValue = GetBitValue(bitIndex);
            bytes[byteIndex] |= bitValue;
        }

        static int GetByteIndex(int bitIndex)
        {
            return bitIndex >> 3;
        }

        static byte GetBitValue(int bitIndex)
        {
            return (byte)(1 << (bitIndex & 7));
        }
    }

    internal class SwitchState
    {
        Label defaultLabel;
        Label endOfSwitchLabel;
        bool defaultDefined;
        internal SwitchState(Label defaultLabel, Label endOfSwitchLabel)
        {
            this.defaultLabel = defaultLabel;
            this.endOfSwitchLabel = endOfSwitchLabel;
            this.defaultDefined = false;
        }
        internal Label DefaultLabel
        {
            get
            {
                return defaultLabel;
            }
        }

        internal Label EndOfSwitchLabel
        {
            get
            {
                return endOfSwitchLabel;
            }
        }
        internal bool DefaultDefined
        {
            get
            {
                return defaultDefined;
            }
            set
            {
                defaultDefined = value;
            }
        }
    }
#endif

    internal class MethodBuilderInfo {
        public readonly MethodBuilder MethodBuilder;
        public readonly Type[] ParameterTypes;
        public MethodBuilderInfo(MethodBuilder methodBuilder, Type[] parameterTypes) {
            this.MethodBuilder = methodBuilder;
            this.ParameterTypes = parameterTypes;
        }

        public void Validate(Type returnType, Type[] parameterTypes, MethodAttributes attributes) {
#if DEBUG
            Debug.Assert(this.MethodBuilder.ReturnType == returnType);
            Debug.Assert(this.MethodBuilder.Attributes == attributes);
            Debug.Assert(this.ParameterTypes.Length == parameterTypes.Length);
            for (int i = 0; i < parameterTypes.Length; ++i) {
                Debug.Assert(this.ParameterTypes[i] == parameterTypes[i]);
            }
#endif
        }
    }

    internal class CodeGeneratorConversionException : Exception {

        private Type sourceType;
        private Type targetType;
        private bool isAddress;
        private string reason;

        public CodeGeneratorConversionException(Type sourceType, Type targetType, bool isAddress, string reason)
            : base() {

            this.sourceType = sourceType;
            this.targetType = targetType;
            this.isAddress = isAddress;
            this.reason = reason;
        }
    }
}
