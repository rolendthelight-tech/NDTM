using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Common
{
	/// <summary>
	/// Инкапсулирует метод без параметров и возвращает значение типа, указанного в параметре <typeparamref name="TResult"/>.
	/// </summary>
	/// <returns>
	/// Возвращаемое значение метода, инкапсулируемого данным делегатом.
	/// </returns>
	/// <typeparam name="TResult">Тип возвращаемого значения метода, инкапсулируемого данным делегатом.</typeparam>
	/// <filterpriority>1</filterpriority>
	[NotNull]
	public delegate TResult FuncNotNull<out TResult>();

	/// <summary>
	/// Инкапсулирует метод с одним параметром и возвращает значение типа, указанного в параметре <typeparamref name="TResult"/>.
	/// </summary>
	/// <returns>
	/// Возвращаемое значение метода, инкапсулируемого данным делегатом.
	/// </returns>
	/// <param name="arg">Параметр метода, инкапсулируемого данным делегатом.</param>
	/// <typeparam name="T">Тип параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="TResult">Тип возвращаемого значения метода, инкапсулируемого данным делегатом.</typeparam>
	/// <filterpriority>2</filterpriority>
	[NotNull]
	public delegate TResult FuncNotNull<in T, out TResult>([NotNull] T arg);

	/// <summary>
	/// Инкапсулирует метод с двумя параметрами и возвращает значение типа, указанного в параметре <typeparamref name="TResult"/>.
	/// </summary>
	/// <returns>
	/// Возвращаемое значение метода, инкапсулируемого данным делегатом.
	/// </returns>
	/// <param name="arg1">Первый параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg2">Второй параметр метода, инкапсулируемого данным делегатом.</param>
	/// <typeparam name="T1">Тип первого параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T2">Тип второго параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="TResult">Тип возвращаемого значения метода, инкапсулируемого данным делегатом.</typeparam>
	/// <filterpriority>2</filterpriority>
	[NotNull]
	public delegate TResult FuncNotNull<in T1, in T2, out TResult>([NotNull] T1 arg1, [NotNull] T2 arg2);

	/// <summary>
	/// Инкапсулирует метод с тремя параметрами и возвращает значение типа, указанного в параметре <typeparamref name="TResult"/>.
	/// </summary>
	/// <returns>
	/// Возвращаемое значение метода, инкапсулируемого данным делегатом.
	/// </returns>
	/// <param name="arg1">Первый параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg2">Второй параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg3">Третий параметр метода, инкапсулируемого данным делегатом.</param>
	/// <typeparam name="T1">Тип первого параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T2">Тип второго параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T3">Тип третьего параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="TResult">Тип возвращаемого значения метода, инкапсулируемого данным делегатом.</typeparam>
	/// <filterpriority>2</filterpriority>
	[NotNull]
	public delegate TResult FuncNotNull<in T1, in T2, in T3, out TResult>([NotNull] T1 arg1, [NotNull] T2 arg2, [NotNull] T3 arg3);

	/// <summary>
	/// Инкапсулирует метод с четырьмя параметрами и возвращает значение типа, указанного в параметре <typeparamref name="TResult"/>.
	/// </summary>
	/// <returns>
	/// Возвращаемое значение метода, инкапсулируемого данным делегатом.
	/// </returns>
	/// <param name="arg1">Первый параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg2">Второй параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg3">Третий параметр метода, инкапсулируемого данным делегатом.</param>
	/// <param name="arg4">Четвёртый параметр метода, инкапсулируемого данным делегатом.</param>
	/// <typeparam name="T1">Тип первого параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T2">Тип второго параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T3">Тип третьего параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="T4">Тип четвёртого параметра метода, инкапсулируемого данным делегатом.</typeparam>
	/// <typeparam name="TResult">Тип возвращаемого значения метода, инкапсулируемого данным делегатом.</typeparam>
	/// <filterpriority>2</filterpriority>
	[NotNull]
	public delegate TResult FuncNotNull<in T1, in T2, in T3, in T4, out TResult>([NotNull] T1 arg1, [NotNull] T2 arg2, [NotNull] T3 arg3, [NotNull] T4 arg4);
}
