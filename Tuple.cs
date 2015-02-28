using UnityEngine;
using System.Collections;

public class Tuple<T1, T2> {
	
	public readonly T1 _1;
	public readonly T2 _2;

	public Tuple(T1 _1, T2 _2) {
		this._1 = _1;
		this._2 = _2;
	}

	public override string ToString() {
		return string.Format("({0}, {1})", _1.ToString(), _2.ToString());
	}

	public override bool Equals(object other) {
		if (!(other is Tuple<T1, T2>)) {
			return false;
		}
		Tuple<T1, T2> o = other as Tuple<T1, T2>;
		return this._1.Equals(o._1) && this._2.Equals(o._2);
	}

	public override int GetHashCode() {
		return this._1.GetHashCode() + 31 * this._2.GetHashCode();
	}
}

public class Tuple {

	public static Tuple<T1, T2> Create<T1, T2>(T1 _1, T2 _2) {
		return new Tuple<T1, T2>(_1, _2);
	}
}
