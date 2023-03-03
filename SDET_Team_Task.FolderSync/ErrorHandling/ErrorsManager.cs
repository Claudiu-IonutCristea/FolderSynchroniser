using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDET_Team_Task.FolderSync.ErrorHandling;
internal static class ErrorsManager
{
	private const string MESSAGES_PATH = @"./ErrorMessages.json";
    private readonly static List<Error> _errors = new();

    public static List<Error> Errors { get => _errors; }
    public static bool HasErrors => _errors.Count > 0;

    public static void Add(int errorCode, string errorSourceName = "")
    {
        _errors.Add(new() { ErrorCode = errorCode, ErrorSource = errorSourceName });
    }

    //Error categories are in multiples of 100
    //check if there are any errors with code between x01 and x99
    public static bool HasErrorFromCategory(ErrorCategory category)
        => _errors.Select(e => e.ErrorCode > (int)category && e.ErrorCode <= (int)category + 99)
                .Any();

    public static void ClearAll()
    {
        _errors.Clear();
    }
}
