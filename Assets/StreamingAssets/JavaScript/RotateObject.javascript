var mTrans = undefined
var rotateVar = undefined

function Awake()
{
	
}

function OnEnable()
{
}

function Start()
{
}

function FixUpdate()
{
}

function Update()
{
	if (mTrans === undefined)
	{
		mTrans = transform;
		rotateVar = new Vector3(0.5, 0.5, 0);
	}
	mTrans.Rotate(rotateVar);
}

function LateUpdate()
{

}

function OnGUI()
{
}

function OnDisable()
{
}

function OnApplicationPause()
{
}

function OnDistroy()
{
}

function OnApplicationQuit()
{
}

function myAdd(a,b)
{
	Debug.Log(a+b);
}