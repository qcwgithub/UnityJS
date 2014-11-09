(function createagameobject()
{
	var v = new GameObject();

	var watch = new Stopwatch();
	watch.Start();
	
	for (var i=0; 100000>i; i++)
	{
		//v.transform.position=Vector3.forward
		v.SetActive(false)
		//v.transform.position=Vector3.forward
		//v.tag="Player"
	}
	watch.Stop();
	printInt(watch.ElapsedMilliseconds)
		v.name="yes i love you"
})()