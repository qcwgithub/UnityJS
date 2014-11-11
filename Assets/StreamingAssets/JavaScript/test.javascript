(function createagameobject()
{
	//var j = new Vector3(0,0,2);
	//return;

	var v = new GameObject();

	var watch = new Stopwatch();
	watch.Start();
	
	for (var i=0; 100000>i; i++)
	{
		//v.transform.position=Vector3.forward
		v.SetActive(false)
		//v.isStatic = false
//		v.transform.position= new Vector3(0,0,2);
		//v.tag="Player"
	}
	watch.Stop();
	printInt(watch.ElapsedMilliseconds)
	//v.name="yes i love you"
})()