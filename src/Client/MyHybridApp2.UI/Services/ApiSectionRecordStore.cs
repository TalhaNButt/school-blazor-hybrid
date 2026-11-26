using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using MyHybridApp2.Contracts.ClassRecords;

namespace MyHybridApp2.UI.Services;

public sealed class ApiSectionRecordStore(HttpClient httpClient) : ISectionRecordStore
{
    public Task<SectionRecordsResponse> GetAsync(
        string schoolId,
        string sectionId,
        DateOnly recordDate,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            var path = $"{GetRecordsPath(sectionId)}?date={recordDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}&schoolId={Uri.EscapeDataString(schoolId)}";
            using var response = await httpClient.GetAsync(path, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await ReadResponseAsync<SectionRecordsResponse>(response, cancellationToken);
        }, "Section records could not be loaded.");

    public Task SaveAttendanceAsync(
        string schoolId,
        string sectionId,
        SaveSectionAttendanceRequest request,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            using var response = await httpClient.PutAsJsonAsync(
                $"{GetRecordsPath(sectionId)}/attendance?schoolId={Uri.EscapeDataString(schoolId)}",
                request,
                cancellationToken);
            response.EnsureSuccessStatusCode();
            await ReadResponseAsync<SectionRecordSaveResponse>(response, cancellationToken);
            return true;
        }, "Attendance could not be saved.");

    public Task SaveDiaryAsync(
        string schoolId,
        string sectionId,
        SaveSectionDiaryRequest request,
        CancellationToken cancellationToken = default) =>
        ExecuteAsync(async () =>
        {
            using var response = await httpClient.PutAsJsonAsync(
                $"{GetRecordsPath(sectionId)}/diary?schoolId={Uri.EscapeDataString(schoolId)}",
                request,
                cancellationToken);
            response.EnsureSuccessStatusCode();
            await ReadResponseAsync<SectionRecordSaveResponse>(response, cancellationToken);
            return true;
        }, "The diary note could not be saved.");

    private static string GetRecordsPath(string sectionId) =>
        $"api/sections/{Uri.EscapeDataString(sectionId)}/records";

    private static async Task<T> ReadResponseAsync<T>(
        HttpResponseMessage response,
        CancellationToken cancellationToken) =>
        await response.Content.ReadFromJsonAsync<T>(cancellationToken)
        ?? throw new JsonException("The section records response was empty.");

    private async Task<T> ExecuteAsync<T>(Func<Task<T>> operation, string errorMessage)
    {
        try
        {
            return await operation();
        }
        catch (HttpRequestException exception)
        {
            var detail = exception.StatusCode is { } statusCode
                ? $"The server returned HTTP {(int)statusCode} ({statusCode})."
                : $"Could not connect to the API at {httpClient.BaseAddress}.";
            throw new InvalidOperationException($"{errorMessage} {detail}", exception);
        }
        catch (JsonException exception)
        {
            throw new InvalidOperationException($"{errorMessage} The server response was invalid.", exception);
        }
    }
}
