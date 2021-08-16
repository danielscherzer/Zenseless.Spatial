namespace Example.Services
{
	public struct Handle<DataType>
	{
		public Handle(int id)
		{
			Id = id;
		}

		public int Id { get; }
	}
}
