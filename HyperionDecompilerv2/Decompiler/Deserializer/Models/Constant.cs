using System;
using System.Collections.Generic;
using System.Text;

namespace HyperionDecompilerv2.Decompiler.Deserializer.Models
{
	public enum Type
	{
		Nil,
		Bool,
		Number,
		String,
		Global
	}

	public abstract class Constant
	{
		public Type Type { get; protected set; }
		public override abstract string ToString();
	}

	public class Constant<T> : Constant
	{
		public T Value { get; private set; }

		protected Constant(Type type, T value)
		{
			Type = type;
			Value = value;
		}

		public override string ToString()
		{
			return Value.ToString();
		}
	}

	public class NilConstant : Constant<object>
	{
		public NilConstant()
			: base(Type.Nil, null)
		{ }

		public override string ToString()
		{
			return "nil";
		}
	}

	public class BoolConstant : Constant<bool>
	{
		public BoolConstant(bool value)
			: base(Type.Bool, value)
		{ }

		public override string ToString()
		{
			return Value ? "true" : "false";
		}
	}

	public class NumberConstant : Constant<double>
	{
		public NumberConstant(double value)
			: base(Type.Number, value)
		{ }
	}

	public class StringConstant : Constant<int>
	{
		public StringConstant(int index)
			: base(Type.String, index)
		{ }
	}

	public class GlobalConstant : Constant<uint>
	{
		public GlobalConstant(uint index)
			: base(Type.Global, index)
		{ }
	}
}
