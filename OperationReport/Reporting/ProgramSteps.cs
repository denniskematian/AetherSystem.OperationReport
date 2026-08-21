using System.Collections.ObjectModel;
using AetherSystem.OperationReport.Entities;

namespace AetherSystem.OperationReport.Reporting;

public sealed class ProgramSteps
{
    public ProgramSteps(
        IReadOnlyList<OperationSample> operationLogs,
        IReadOnlyList<OperationLogLabel> operationLogLabels)
    {
        ArgumentNullException.ThrowIfNull(operationLogs);
        ArgumentNullException.ThrowIfNull(operationLogLabels);

        foreach (var operationLog in operationLogs)
            ArgumentNullException.ThrowIfNull(operationLog);
        
        foreach (var label in operationLogLabels)
            ArgumentNullException.ThrowIfNull(label);

        if (operationLogLabels.Any(label => label.Id < 0))
            throw new ArgumentOutOfRangeException(
                nameof(operationLogLabels),
                "Operation log label IDs cannot be negative.");

        if (operationLogLabels.Select(label => label.Id).Distinct().Count() != operationLogLabels.Count)
            throw new ArgumentException(
                "Operation log label IDs must be unique.",
                nameof(operationLogLabels));

        foreach (var log in operationLogs)
        {
            var invalidLabel = operationLogLabels.FirstOrDefault(label => label.Id >= log.Values.Count);
            if (invalidLabel is not null)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(operationLogLabels),
                    invalidLabel.Id,
                    $"Label ID {invalidLabel.Id} is outside an operation log value range.");
            }
        }

        OperationLogs = operationLogs;
        OperationLogLabels = operationLogLabels;
        MinValues = Calculate(operationLogLabels, operationLogs, values => values.Min());
        MaxValues = Calculate(operationLogLabels, operationLogs, values => values.Max());
        AvgValues = Calculate(operationLogLabels, operationLogs, values => values.Average());
    }

    public IReadOnlyList<OperationSample> OperationLogs { get; }
    public IReadOnlyList<OperationLogLabel> OperationLogLabels { get; }
    public IReadOnlyList<double> MinValues { get; }
    public IReadOnlyList<double> MaxValues { get; }
    public IReadOnlyList<double> AvgValues { get; }

    private static ReadOnlyCollection<double> Calculate(
        IReadOnlyList<OperationLogLabel> labels,
        IReadOnlyList<OperationSample> logs,
        Func<IEnumerable<double>, double> aggregate)
    {
        if (logs.Count == 0)
            return Array.AsReadOnly(labels.Select(_ => double.NaN).ToArray());

        var result = labels
            .Select(label => aggregate(logs.Select(log => log.Values[label.Id])))
            .ToArray();

        return Array.AsReadOnly(result);
    }
}