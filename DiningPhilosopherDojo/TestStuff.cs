using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Schema;
using NUnit.Framework;
using Timer = System.Timers.Timer;

namespace DiningPhilosopherDojo
{
	[TestFixture]
	public class TestStuff
	{
		private Chopstick _chopstick;
		private Philosopher _philosopher;

		[SetUp]
		public void Setup()
		{
			_chopstick = new Chopstick(1);
			_philosopher = new Philosopher(1);
        }

		[Test]
		public void IsAvailable_GivenNewChopstick_ReturnsTrue()
		{
			Assert.That(_chopstick.IsAvailable, Is.True);
		}

		[Test]
		public void IsAvailable_GivenChopstickIsPickedUp_ReturnsFalse()
		{
			_chopstick.PickUp();
			Assert.That(_chopstick.IsAvailable, Is.False);
		}

		[Test]
		public void IsAvailable_GivenPickedUpChopstickPutDown_ReturnsTrue()
		{
			_chopstick.PickUp();
			_chopstick.PutDown();
			Assert.That(_chopstick.IsAvailable, Is.True);
		}

		[Test]
		public void CanEat_GivenBothAvailableChopSticks_ReturnsTrue()
		{
			Assert.That(_philosopher.CanEat, Is.True);
		}

		[Test]
		public void CanEat_GivenOneAvailableChopStick_ReturnsFalse()
		{
			_philosopher.LeftChopstick.PickUp();
			Assert.That(_philosopher.CanEat, Is.False);
		}

		[Test]
		public void CanEat_GivenNoAvailableChopSticks_ReturnsFalse()
		{
			_philosopher.Eat();
			Assert.That(_philosopher.CanEat, Is.False);
		}

		[Test]
		public void Eat_GivenBothAvailableChopsticks_ReturnsTrue()
		{
			Assert.That(_philosopher.Eat(), Is.True);
		}

		[Test]
		public void Eat_GivenTwoPhilosophersAndTwoChopSticksWhenPhilosopherOneEatsPhilosopher2Cannot_ReturnsFalse()
		{
			var chopstick1 = new Chopstick(1);
			var chopstick2 = new Chopstick(2);

			var philosopher1 = new Philosopher(1,chopstick1, chopstick2);
			var philosopher2 = new Philosopher(2, chopstick2, chopstick1);

			philosopher1.Eat();

			Assert.That(philosopher2.Eat(), Is.False);
		}

		[Test]
		public void Meal_ListPhilosophersAllEat_Returns()
		{
			var chopstick1 = new Chopstick(1);
			var chopstick2 = new Chopstick(2);
			var chopstick3 = new Chopstick(3);
			var chopstick4 = new Chopstick(4);
			var chopstick5 = new Chopstick(5);

			var philosophers = new List<Philosopher>
			{
				new Philosopher(1, chopstick1, chopstick2),
				new Philosopher(2, chopstick2, chopstick3),
				new Philosopher(3, chopstick3, chopstick4),
				new Philosopher(4, chopstick4, chopstick5),
				new Philosopher(5, chopstick5, chopstick1)
			};
			Parallel.ForEach(philosophers,new ParallelOptions { MaxDegreeOfParallelism = 5}, philosopher =>
			{
				philosopher.Consume();
			});
		}
	}

	public class Chopstick
	{
		private int _id;

		public bool IsAvailable { get; private set; }

		public Chopstick()
		{
			IsAvailable = true;
		}

		public Chopstick(int id)
		{
			_id = id;
			IsAvailable = true;
		}

		public void PickUp()
		{
			
			IsAvailable = false;
		}

		public void PutDown()
		{
			IsAvailable = true;
		}
	}

	public class Philosopher
	{
		public Chopstick LeftChopstick { get; set; }
		public Chopstick RightChopstick { get; set; }
		private int _id;
		private Random random = new Random();

		public bool CanEat => LeftChopstick.IsAvailable && RightChopstick.IsAvailable;

		public Philosopher(int id)
		{
			_id = id;
			LeftChopstick = new Chopstick();
			RightChopstick = new Chopstick();
		}

		public Philosopher(int id, Chopstick leftChopstick, Chopstick rightChopstick)
		{
			LeftChopstick = leftChopstick;
			RightChopstick = rightChopstick;
			_id = id;
		}

		public bool Eat()
		{
			if (LeftChopstick.IsAvailable)
			{
				lock (LeftChopstick)
				{
					LeftChopstick.PickUp();
					if (RightChopstick.IsAvailable)
					{
						lock (RightChopstick)
						{
							RightChopstick.PickUp();
							Console.WriteLine("I am eating, said Philospher: {0}", _id);
							RightChopstick.PutDown();
							LeftChopstick.PutDown();
							return true;
						}
					}
					LeftChopstick.PutDown();
				}
			}
			return false;
		}

		public void Consume()
		{
			int bites = 0;
			while (bites < 100)
			{
				if (Eat())
				{
					bites++;
				}
			}
		}
	}
}
