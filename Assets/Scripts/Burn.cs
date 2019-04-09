using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burn : MonoBehaviour {
    void Start() {
    }

    void Update() {
    }

    public void DestroyThis() {
        Destroy(this.gameObject);
    }
}