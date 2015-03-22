using UnityEngine;
using System.Collections;

public static class LocalInteractionUtilities {

	public static readonly Vector3 forwardLeft = new Vector3(-1, 0, 1).normalized;
	public static readonly Vector3 forwardRight = new Vector3(1, 0, 1).normalized;
	public static readonly Vector3 backLeft = new Vector3(-1, 0, -1).normalized;
	public static readonly Vector3 backRight = new Vector3(1, 0, -1).normalized;
	

	// Decides move for point models
	public static Vector3 DecidePointMove() {
		bool w = Input.GetKey("w");
		bool a = Input.GetKey("a");
		bool s = Input.GetKey("s");
		bool d = Input.GetKey("d");
		
		if (w && s || a && d) { return Vector3.zero; }
		if (a && w) { return forwardLeft;  }
		if (a && s) { return backLeft;     }
		if (s && d) { return backRight;    }
		if (d && w) { return forwardRight; }
		if (a) { return Vector3.left;    }
		if (s) { return Vector3.back;    }
		if (d) { return Vector3.right;   }
		if (w) { return Vector3.forward; }

		return Vector3.zero;
	}
}
