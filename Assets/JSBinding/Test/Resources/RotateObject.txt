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

    //
    var arrObjs = GameObject.FindGameObjectsWithTag('Respawn')
    for (var i = 0; i < arrObjs.length; i++)
        Debug.Log("Respawn: " + arrObjs[i].name);



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
    var arr = k.getArr();
    for(var i = 0; i < arr.length; i++)
        Debug.Log(arr[i]);

    // not supported now.
    // k.inputIntArr([999,888,777,666,555])
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
        
        //if (t !== undefined)
        //{
        //    Debug.Log('Hit ' + t.name);
        //}
        if (t.Equals(mTrans))
        {
            Debug.Log('Hit ' + t.name);

            var prefab = GameObject.Find('CubeInit');
            if (prefab === undefined)
            {
                Debug.Log('prefab is undefined!');
                return;
            }
            var newGo = UnityObject.Instantiate(prefab);
            newGo.SetActive(true);
            var tn = newGo.transform;
            tn.localScale = new Vector3(0.3,0.3,0.3);
            tn.position = new Vector3(Random.Range(-2.1,2.1), Random.Range(-1.1,1.1), -7.24)
            CS.AddJSComponent(newGo, "RotateObject2");

        }
    }

    // destroy this GameObject after 10 seconds
    fAccum += Time.deltaTime
    if (fAccum > 10)
    {
        // JavaScript has 'Object' as key word
        // so UnityEngine.Object is renamed to 'UnityObject'
        //UnityObject.Destroy(this);
    }
}

function Destroy()
{
    Debug.Log('js Destroy Called!')
}

function OnGUI()
{
    GUILayout.TextArea('Click the big cube!', null)
}
