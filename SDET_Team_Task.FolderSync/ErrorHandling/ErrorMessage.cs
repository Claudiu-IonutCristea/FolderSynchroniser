using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SDET_Team_Task.FolderSync.ErrorHandling;
internal class ErrorMessage
{
	[JsonPropertyName("id")]
	public int ErrorId { get; set; }
	[JsonPropertyName("message")]
	public string Message { get; set; } = "";
}
