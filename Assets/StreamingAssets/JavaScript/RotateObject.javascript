/*
* this is a simple callback js
* functions (except delegate function) are called with 'this' set to a GameObject
*/

// these 2 variables belongs to 'this' object
// 

//globalObj = new GameObject('JZYX')

var mTrans = undefined
var rotateVar = undefined
var fAccum = 0

function Awake()
{
    // var v = new GameObject('Micheal Jackson');
    //mTrans = transform;

    //Debug.Log('Awake called');
}
function Start()
{
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
		var v2 = new Vector3(0.5,0.5,0)
		var k1 = new Kekoukele();
		var k2 = new Kekoukele();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
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
        }
    }

    fAccum += Time.deltaTime
    if (fAccum > 10)
    {
       UnityObject.Destroy(this);
    }
}

function Destroy()
{
    Debug.Log('js Destroy Called!')
}