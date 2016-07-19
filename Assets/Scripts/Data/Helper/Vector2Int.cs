// Vector3Int.cs
//
// Author: Jate Wittayabundit <jate@ennface.com>
using UnityEngine;

[System.Serializable]
public struct Vector2Int {
    public int x;
    public int y;
    
    public static Vector2Int zero {
        get { return new Vector2Int(0,0); }
    }
    
    public static Vector2Int one {
        get { return new Vector2Int(1,1); }
    }
    
    public static Vector2Int operator +(Vector2Int c1, Vector2Int c2) {
        return new Vector2Int(c1.x + c2.x, c1.y + c2.y);
    }
    public static Vector2Int operator -(Vector2Int c1, Vector2Int c2) {
        return new Vector2Int(c1.x - c2.x, c1.y - c2.y);
    }
    
    public override int GetHashCode() {
       return base.GetHashCode() ^ y;
    }

    // Override the Object.Equals(object o) method:
    public override bool Equals(object o) {
        try {
            Vector2Int other = (Vector2Int)o;
            if ((this.x == other.x) && (this.x == other.y)) {
                return true;
            }
            return false;
        } catch {
            return false;
        }
    }
    
    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b))
        {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null))
        {
            return false;
        }

        // Return true if the fields match:
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return !(a == b);
    }

    // Override the ToString method to convert DBBool to a string:
    public override string ToString() {
        return "x="+x+",y="+y;
    }
    
    public Vector2 ToVector2 {
        get {
            return new Vector2(x,y);
        }
    }
    
    public Vector2Int (int x, int y) {
        this.x = x;
        this.y = y;
    }
    
    public Vector2Int (Vector2Int vector) {
        this.x = vector.x;
        this.y = vector.y;
    }
    
    public Vector2Int (Vector2 vector) {
        this.x = (int)vector.x;
        this.y = (int)vector.y;
    }
}