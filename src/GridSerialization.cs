using System.Text.Json;

namespace Zenseless.Spatial
{
	/// <summary>
	/// Contains helper methods to do deserialization for JSON. Serialization works the defualt way with <seealso cref="JsonSerializer.Serialize(object?, System.Type, JsonSerializerOptions?)"/>
	/// </summary>
	public static class GridSerialization
	{
		private struct Helper<CellType>
		{
			public CellType[] AsReadOnly { get; set; }
			public int Columns { get; set; }
			public int Rows { get; set; }
		}

		/// <summary>
		/// Deserialize a <seealso cref="Grid{CellType}"/> from a JSON string
		/// </summary>
		/// <typeparam name="CellType">Cell data type</typeparam>
		/// <param name="jsonString">a JSON string that contains a serialized instance of <seealso cref="Grid{CellType}"/></param>
		/// <returns>A new instance of <seealso cref="Grid{CellType}"/></returns>
		public static Grid<CellType> Deserialize<CellType>(string jsonString)
		{
			var helper = JsonSerializer.Deserialize<Helper<CellType>>(jsonString);
			Grid<CellType> grid = new(helper.Columns, helper.Rows);
			grid.CopyFrom(helper.AsReadOnly, grid.AsReadOnly.Count);
			return grid;
		}
	}
}
