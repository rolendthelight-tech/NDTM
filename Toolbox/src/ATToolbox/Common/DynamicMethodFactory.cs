using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;

namespace AT.Toolbox
{
  /// <summary>
  /// A helper class that uses lightweight code generation to create dynamic methods.
  /// </summary>

  public delegate object FactoryMethod(params object[] arguments);
  public delegate object Invoker(object target, params object[] arguments);

  public static class DynamicMethodFactory
  {
    /*----------------------------------------------------------------------------------------*/
    #region Static Fields
    private static readonly ConstructorInfo TargetParameterCountExceptionConstructor =
        typeof(TargetParameterCountException).GetConstructor(Type.EmptyTypes);
    private static readonly Module Module = typeof(DynamicMethodFactory).Module;
    #endregion
    /*----------------------------------------------------------------------------------------*/
    #region Public Methods
    /// <summary>
    /// Creates a new <see cref="Invoker"/> that calls the specified method in a
    /// late-bound manner.
    /// </summary>
    /// <param name="method">The method that the invoker should call.</param>
    /// <returns>A dynamic invoker that can call the specified method.</returns>
    public static Invoker CreateInvoker(this MethodInfo method)
    {
      DynamicMethod callable = CreateDynamicInvoker();
      var info = new DelegateBuildInfo(method);

      ILGenerator il = callable.GetILGenerator();

      EmitCheckParameters(info, il, 1);
      EmitLoadParameters(info, il, 1);

      if (method.IsStatic)
        il.EmitCall(OpCodes.Call, method, null);
      else
        il.EmitCall(OpCodes.Callvirt, method, null);

      if (method.ReturnType == typeof(void))
      {
        il.Emit(OpCodes.Ldnull);
      }
      else
      {
        if (info.ReturnType.IsValueType)
          il.Emit(OpCodes.Box, method.ReturnType);
      }

      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(Invoker)) as Invoker;
    }
    /*----------------------------------------------------------------------------------------*/
    /// <summary>
    /// Creates a new <see cref="FactoryMethod"/> that calls the specified constructor in a
    /// late-bound manner.
    /// </summary>
    /// <param name="constructor">The constructor that the factory method should call.</param>
    /// <returns>A dynamic factory method that can call the specified constructor.</returns>
    public static FactoryMethod CreateFactoryMethod(ConstructorInfo constructor)
    {
      DynamicMethod callable = CreateDynamicFactoryMethod();
      var info = new DelegateBuildInfo(constructor);

      Type returnType = constructor.ReflectedType;
      ILGenerator il = callable.GetILGenerator();

      EmitCheckParameters(info, il, 0);
      EmitLoadParameters(info, il, 0);

      il.Emit(OpCodes.Newobj, constructor);

      if (info.ReturnType.IsValueType)
        il.Emit(OpCodes.Box, returnType);

      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(FactoryMethod)) as FactoryMethod;
    }
    /*----------------------------------------------------------------------------------------*/
    /// <summary>
    /// Creates a new <see cref="Getter"/> that gets the value of the specified field in a
    /// late-bound manner.
    /// </summary>
    /// <param name="field">The field that the getter should read from.</param>
    /// <returns>A dynamic getter that can read from the specified field.</returns>
    public static Func<object, object> CreateGetter(this FieldInfo field)
    {
      DynamicMethod callable = CreateDynamicGetterMethod();

      Type returnType = field.FieldType;
      ILGenerator il = callable.GetILGenerator();

      il.Emit(OpCodes.Ldarg_0);
      EmitUnboxOrCast(il, field.DeclaringType);
      il.Emit(OpCodes.Ldfld, field);

      if (returnType.IsValueType)
        il.Emit(OpCodes.Box, returnType);

      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(Func<object, object>)) as Func<object, object>;
    }
    /*----------------------------------------------------------------------------------------*/
    /// <summary>
    /// Creates a new <see cref="Getter"/> that gets the value of the specified property in a
    /// late-bound manner.
    /// </summary>
    /// <param name="property">The property that the getter should read from.</param>
    /// <returns>A dynamic getter that can read from the specified property.</returns>
    public static Func<object, object> CreateGetter(this PropertyInfo property)
    {
      DynamicMethod callable = CreateDynamicGetterMethod();

      Type returnType = property.PropertyType;
      ILGenerator il = callable.GetILGenerator();
      MethodInfo method = property.GetGetMethod(true);

      il.Emit(OpCodes.Ldarg_0);
      EmitUnboxOrCast(il, property.DeclaringType);

      if (method.IsFinal)
        il.Emit(OpCodes.Call, method);
      else
        il.Emit(OpCodes.Callvirt, method);

      if (returnType.IsValueType)
        il.Emit(OpCodes.Box, returnType);

      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(Func<object, object>)) as Func<object, object>;
    }
    /*----------------------------------------------------------------------------------------*/
    /// <summary>
    /// Creates a new <see cref="Setter"/> that sets the value of the specified field in a
    /// late-bound manner.
    /// </summary>
    /// <param name="field">The field that the setter should write to.</param>
    /// <returns>A dynamic setter that can write to the specified field.</returns>
    public static Action<object, object> CreateSetter(this FieldInfo field)
    {
      DynamicMethod callable = CreateDynamicSetterMethod();

      Type returnType = field.FieldType;
      ILGenerator il = callable.GetILGenerator();

      il.DeclareLocal(returnType);

      il.Emit(OpCodes.Ldarg_1);
      EmitUnboxOrCast(il, returnType);
      il.Emit(OpCodes.Stloc_0);

      il.Emit(OpCodes.Ldarg_0);
      EmitUnboxOrCast(il, field.DeclaringType);
      il.Emit(OpCodes.Ldloc_0);

      il.Emit(OpCodes.Stfld, field);
      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(Action<object, object>)) as Action<object, object>;
    }
    /*----------------------------------------------------------------------------------------*/
    /// <summary>
    /// Creates a new <see cref="Setter"/> that sets the value of the specified property in a
    /// late-bound manner.
    /// </summary>
    /// <param name="property">The property that the setter should write to.</param>
    /// <returns>A dynamic setter that can write to the specified property.</returns>
    public static Action<object, object> CreateSetter(this PropertyInfo property)
    {
      DynamicMethod callable = CreateDynamicSetterMethod();

      Type returnType = property.PropertyType;
      ILGenerator il = callable.GetILGenerator();
      MethodInfo method = property.GetSetMethod(true);

      il.DeclareLocal(returnType);

      il.Emit(OpCodes.Ldarg_1);
      EmitUnboxOrCast(il, returnType);
      il.Emit(OpCodes.Stloc_0);

      il.Emit(OpCodes.Ldarg_0);
      EmitUnboxOrCast(il, property.DeclaringType);
      il.Emit(OpCodes.Ldloc_0);

      if (method.IsFinal)
        il.Emit(OpCodes.Call, method);
      else
        il.Emit(OpCodes.Callvirt, method);

      il.Emit(OpCodes.Ret);

      return callable.CreateDelegate(typeof(Action<object, object>)) as Action<object, object>;
    }
    #endregion
    /*----------------------------------------------------------------------------------------*/
    #region Private Methods: DynamicMethod Creation
    private static DynamicMethod CreateDynamicInvoker()
    {
#if !NO_SKIP_VISIBILITY
      return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object), typeof(object[]) }, Module, true);
#else
            return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object), typeof(object[]) });
#endif
    }
    /*----------------------------------------------------------------------------------------*/
    private static DynamicMethod CreateDynamicFactoryMethod()
    {
#if !NO_SKIP_VISIBILITY
      return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object[]) }, Module, true);
#else
            return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object[]) });
#endif
    }
    /*----------------------------------------------------------------------------------------*/
    private static DynamicMethod CreateDynamicGetterMethod()
    {
#if !NO_SKIP_VISIBILITY
      return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object) }, Module, true);
#else
            return new DynamicMethod(String.Empty, typeof(object), new[] { typeof(object) });
#endif
    }
    /*----------------------------------------------------------------------------------------*/
    private static DynamicMethod CreateDynamicSetterMethod()
    {
#if !NO_SKIP_VISIBILITY
      return new DynamicMethod(String.Empty, typeof(void), new[] { typeof(object), typeof(object) }, Module, true);
#else
            return new DynamicMethod(String.Empty, typeof(void), new[] { typeof(object), typeof(object) });
#endif
    }
    #endregion
    /*----------------------------------------------------------------------------------------*/
    #region Private Methods: Utility
    private static void EmitCheckParameters(DelegateBuildInfo info, ILGenerator il, int argumentArrayIndex)
    {
      Label beginLabel = il.DefineLabel();

      EmitLoadArg(il, argumentArrayIndex);
      il.Emit(OpCodes.Ldlen);
      EmitLoadInt(il, info.Parameters.Length);
      il.Emit(OpCodes.Beq, beginLabel);

      il.Emit(OpCodes.Newobj, TargetParameterCountExceptionConstructor);
      il.Emit(OpCodes.Throw);

      il.MarkLabel(beginLabel);
    }
    /*----------------------------------------------------------------------------------------*/
    private static void EmitLoadParameters(DelegateBuildInfo info, ILGenerator il, int argumentArrayIndex)
    {
      if (!info.Method.IsStatic && !(info.Method is ConstructorInfo))
      {
        il.Emit(OpCodes.Ldarg_0);
        EmitUnboxOrCast(il, info.Method.DeclaringType);
      }

      for (int index = 0; index < info.Parameters.Length; index++)
      {
        EmitLoadArg(il, argumentArrayIndex);
        EmitLoadInt(il, index);
        il.Emit(OpCodes.Ldelem_Ref);
        EmitUnboxOrCast(il, info.ParameterTypes[index]);
      }
    }
    /*----------------------------------------------------------------------------------------*/
    private static Type[] GetActualParameterTypes(System.Reflection.ParameterInfo[] parameters)
    {
      Type[] types = new Type[parameters.Length];

      for (int index = 0; index < parameters.Length; index++)
      {
        Type type = parameters[index].ParameterType;
        types[index] = (type.IsByRef ? type.GetElementType() : type);
      }

      return types;
    }
    /*----------------------------------------------------------------------------------------*/
    private static void EmitUnboxOrCast(ILGenerator il, Type type)
    {
      if (type.IsValueType)
        il.Emit(OpCodes.Unbox_Any, type);
      else
        il.Emit(OpCodes.Castclass, type);
    }
    /*----------------------------------------------------------------------------------------*/
    private static void EmitLoadInt(ILGenerator il, int value)
    {
      switch (value)
      {
        case -1:
          il.Emit(OpCodes.Ldc_I4_M1);
          break;
        case 0:
          il.Emit(OpCodes.Ldc_I4_0);
          break;
        case 1:
          il.Emit(OpCodes.Ldc_I4_1);
          break;
        case 2:
          il.Emit(OpCodes.Ldc_I4_2);
          break;
        case 3:
          il.Emit(OpCodes.Ldc_I4_3);
          break;
        case 4:
          il.Emit(OpCodes.Ldc_I4_4);
          break;
        case 5:
          il.Emit(OpCodes.Ldc_I4_5);
          break;
        case 6:
          il.Emit(OpCodes.Ldc_I4_6);
          break;
        case 7:
          il.Emit(OpCodes.Ldc_I4_7);
          break;
        case 8:
          il.Emit(OpCodes.Ldc_I4_8);
          break;
        default:
          if (value > -129 && value < 128)
            il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
          else
            il.Emit(OpCodes.Ldc_I4, value);
          break;
      }
    }
    /*----------------------------------------------------------------------------------------*/
    private static void EmitLoadArg(ILGenerator il, int index)
    {
      switch (index)
      {
        case 0:
          il.Emit(OpCodes.Ldarg_0);
          break;
        case 1:
          il.Emit(OpCodes.Ldarg_1);
          break;
        case 2:
          il.Emit(OpCodes.Ldarg_2);
          break;
        case 3:
          il.Emit(OpCodes.Ldarg_3);
          break;
        default:
          if (index > -129 && index < 128)
            il.Emit(OpCodes.Ldarg_S, (sbyte)index);
          else
            il.Emit(OpCodes.Ldarg, index);
          break;
      }
    }
    #endregion
    /*----------------------------------------------------------------------------------------*/
    #region Inner Types
    private class DelegateBuildInfo
    {
      public MethodBase Method { get; private set; }
      public Type ReturnType { get; private set; }
      public System.Reflection.ParameterInfo[] Parameters { get; private set; }
      public Type[] ParameterTypes { get; private set; }

      public DelegateBuildInfo(ConstructorInfo ctor)
      {
        Method = ctor;
        ReturnType = ctor.ReflectedType;
        InitParameters();
      }

      public DelegateBuildInfo(MethodInfo method)
      {
        Method = method;
        ReturnType = method.ReturnType;
        InitParameters();
      }

      private void InitParameters()
      {
        Parameters = Method.GetParameters();
        ParameterTypes = GetActualParameterTypes(Parameters);
      }
    }
    #endregion
    /*----------------------------------------------------------------------------------------*/
  }
}
