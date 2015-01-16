/*

	PhysicsJS Test
	Alex Norton
	2015

*/


$(document).ready(function(){
	// world declaration
	var world = Physics();
	// creation of the renderer which will draw the world
	var renderer = Physics.renderer("canvas",{
			el: "canvasid",	// canvas element id
		width: 640,		// canvas width
		height: 480,		// canvas height
		meta: false		// setting it to "true" will display FPS
	});	
	// adding the renderer to the world
	world.add(renderer);
	// what happens at every iteration step? We render (show the world)
	world.subscribe("step",function(){
			world.render();
	});
	// this is the default gravity
	var gravity = Physics.behavior("constant-acceleration",{
			acc: {
			x:0, 
			y:0.0004
		} 
	});
	// adding gravity to the world
	world.add(gravity);
	// adding collision detection with canvas edges
	world.add(Physics.behavior("edge-collision-detection", {
			aabb: Physics.aabb(0, 0, 640, 480),
			restitution: 0
		}));
		// bodies will react to forces such as gravity
		world.add(Physics.behavior("body-impulse-response"));
		// enabling collision detection among bodies
		world.add(Physics.behavior("body-collision-detection"));
	world.add(Physics.behavior("sweep-prune"));
     $("#canvasid").click(function(e){
     	// checking canvas coordinates for the mouse click
		var offset = $(this).offset();
		var px = e.pageX - offset.left;
			var py = e.pageY - offset.top;
			// this is the way physicsjs handles 2d vectors, similar at Box2D's b2Vec
		var mousePos = Physics.vector();
			mousePos.set(px,py);
			// finding a body under mouse position
			var body = world.findOne({
			$at: mousePos
		})

		// some fun colors
    	var colors = [
	        '#b58900',
	        '#cb4b16',
	        '#dc322f',
	        '#d33682',
	        '#6c71c4',
	        '#268bd2',
	        '#2aa198',
	        '#859900'
    	];
		// there isn't any body under mouse position, going to create a new box
		if(!body){
	     	world.add(Physics.body("convex-polygon",{
					x: px,
					y: py,
					vertices: [
						{x:0, y:0},
						{x:0, y:60},
						{x:60, y:60},
						{x:60, y:0}

					],
					restitution:0.5,
					styles: {
                        fillStyle: colors[ 0 ]
                    }
				}));
		}
		else{
			// there is a body under mouse position, let's remove it
			world.removeBody(body);
		} 
	})
	// handling timestep
		Physics.util.ticker.subscribe(function(time,dt){
			world.step(time);
		});
	Physics.util.ticker.start();
})