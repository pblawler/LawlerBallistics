Lawler Ballistics

Fo:
1.  Provided Velocities and distances are used first to calculate Fo and Muzzle velocity.
If muzzle velocity is one of the provided velocities then the associated distance (D1) will be zero.
This method will always be applied first and overide other values as actual velocity and distance
measurements are considered more accurate than advertised BCs and muzzle velocities.

2.  If velocities and distances are not provided the BCg1 and provided Muzzle Velocity will be used
to calculate Fo and V1, D1, V2, and D2.

F2:
1.  Provided F2 value will be used, otherwise F2 will be calculated.  Any drop data provided will be used to
correct the F2 value.

F3:
1.  Provided F3 value will be used, otherwise F3 will be calculated.  Any drop data provided will be used to
correct the F3 value.

Zero Latitude and Longitude:
	During zero the coordinates for the shooter and target are used to get the direction of fire for the
	coriolis effect calculations.  The distance used is the provided range which is typically aquired with
	a laser range finder that is usually more accurate than most devices' gps.