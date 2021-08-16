//using OpenTK.Mathematics;
//using System;
//using System.Collections.Generic;

//namespace Example
//{
//	public class KdTree<DataType> where DataType : IPosition
//	{
//		public KdTree(int axis, Box2 bounds)
//		{
//			Axis = axis;
//			Bounds = bounds;
//		}

//		public int Axis { get; }
//		public Box2 Bounds { get; }
		
//		public const uint Capacity = 3;

//		public IEnumerable<DataType> Item => _items;

//		public bool Insert(DataType item)
//		{
//			if (_items.Count < Capacity)
//			{
//				_items.Add(item);
//			}
//			else
//			{
//				if (_childSmall is null) Subdivide();
				
//				foreach (var child in _children)
//				{
//					if (child.Insert(item)) return true;
//				}
//			}
//			return true;
//		}

//		private readonly List<DataType> _items = new();
//		private KdTree<DataType> _childSmall;
//		private KdTree<DataType> _childBig;

//		private void Subdivide()
//		{
//			var boundSmall = new Box2(Bounds.Min, Bounds.Min + Bounds.ha
//			var newAxis = (Axis + 1) % 2;
//			_childSmall = new KdTree<DataType>(newAxis, );
//		}
//	}
//}
