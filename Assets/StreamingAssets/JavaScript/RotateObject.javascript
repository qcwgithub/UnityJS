/*
* this is a simple callback js
* functions (except delegate function) are called with 'this' set to a GameObject
*/

// these variables belongs to 'this' object
// 
var mTrans = undefined
var rotateVar = undefined
var fAccum = 0

function Awake()
{   
    // example to test value type
    // expected output is 100,66666
    // also demostrate to call function with object parameter
    var v1 = Vector3.one
    var v2 = Vector3.one
    v1.x = 100
    v2.x = 66666
    Debug.Log(v1.x);
    Debug.Log(v2.x);
}
function Start()
{
    // expample to call out parameter by wrapping it
    // expected output is 899
    // 'CS' is defined in c#
    var k = new Kekoukele();
    var v = CS.int32(0);
    k.getValue(v);
    Debug.Log(v.Value())
}

function Update()
{
    // here
    // 'this' means a GameObject

    // rotate gameobject
    if (mTrans === undefined)
    {
        mTrans = transform;
        rotateVar = new Vector3(0.5, 0.5, 0);
    }
    mTrans.Rotate(rotateVar);

    if (Input.GetMouseButtonDown(0) === true)
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // call function with out struct
        // must call new to create object at first
        var hit = new RaycastHit();
        Physics.Raycast(ray, hit);
        var t = hit.transform;

        if (t === undefined)
        {
            Debug.Log('Hit nothing')
        }
        else
        {
            Debug.Log('Hit ' + t.name);

            var newGo = new GameObject("Flower");
            CS.AddJSComponent(newGo, "RotateObject2");
        }
    }

    // destroy this GameObject after 10 seconds
    fAccum += Time.deltaTime
    if (fAccum > 10)
    {
        // JavaScript has 'Object' as key word
        // so UnityEngine.Object is renamed to 'UnityObject'
        UnityObject.Destroy(this);
    }
}

function Destroy()
{
    Debug.Log('js Destroy Called!')
}