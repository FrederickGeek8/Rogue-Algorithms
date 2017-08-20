using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Tuple : IEquatable<Tuple> {
	public int[] values;

	public Tuple (int[] toSet) {
		values = toSet;
	}

	public Tuple Clone() {
		return new Tuple ((int[]) values.Clone ());
	}

	public bool Equals(Tuple other) {
		return values.SequenceEqual(other.values);
	}

	public override bool Equals(Object obj) {
		Tuple other = obj as Tuple;
		return other != null && Equals (other);
	}

	public override int GetHashCode () {
		return values.Aggregate(0, (acc, i) => unchecked(acc * 457 + i * 389));
	}
}

