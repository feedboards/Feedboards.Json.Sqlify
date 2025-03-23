# TODO List

## Exception Usage Analysis

### SQL_001 Error Code Issues

1. The `SQL_001` error code is defined but not properly implemented:

   - Defined in `ErrorCodes.cs` as `NestedStructureLimit = "SQL_001"`
   - Documentation exists in both `errors_code.md` and `error_types.md`
   - However, the code never throws this specific error code variant

2. Current `NestedStructureLimitException` implementation:
   - Missing SQL-specific metadata fields (`TableName`, `NestedField`)
   - No distinction between JSON nesting limits and SQL nesting limits

### Required Improvements

1. **ClickHouseSQLBuilder.cs**:

   - Add SQL nesting depth validation in `GenerateClickHouseSchema` method
   - Throw `NestedStructureLimitException` with `SQL_001` when SQL nesting exceeds limits
   - Add tracking of nested field names for better error reporting

2. **NestedStructureLimitException.cs**:

   - Add constructor overload for SQL-specific metadata
   - Add support for `TableName` and `NestedField` metadata fields
   - Implement proper error code selection based on context

3. **Test Coverage**:

   - Add tests specifically for SQL nesting limits
   - Test both JSON (`JSN_002`) and SQL (`SQL_001`) nesting scenarios
   - Verify correct metadata is included in exceptions

4. **Documentation Updates**:
   - Clarify the difference between JSON and SQL nesting limits
   - Update examples to show both types of nesting limit exceptions
   - Add migration guide for users of the current implementation

## Code Examples

### Suggested SQL Nesting Check Implementation

```csharp
private void ValidateSQLNesting(Dictionary<string, string> structure, string tableName, int maxDepth)
{
    foreach (var kvp in structure)
    {
        var nestedCount = kvp.Value.Split(new[] { "Nested(" }, StringSplitOptions.None).Length - 1;
        if (maxDepth > 0 && nestedCount > maxDepth)
        {
            throw new NestedStructureLimitException(
                actualDepth: nestedCount,
                maxAllowedDepth: maxDepth,
                tableName: tableName,
                nestedField: kvp.Key);
        }
    }
}
```

### Suggested Exception Constructor

```csharp
public NestedStructureLimitException(
    int? actualDepth = null,
    int? maxAllowedDepth = null,
    string? tableName = null,
    string? nestedField = null,
    Exception? innerException = null)
    : base(
        errorCode: tableName != null ? ErrorCodes.NestedStructureLimit : ErrorCodes.InvalidJsonStructure,
        message: BuildMessage(actualDepth, maxAllowedDepth, tableName, nestedField),
        innerException: innerException,
        metadata: BuildMetadata(actualDepth, maxAllowedDepth, tableName, nestedField))
{
}
```

## Priority Order

1. High Priority:

   - Implement SQL nesting validation
   - Update NestedStructureLimitException to support both error codes
   - Add basic test coverage for SQL nesting limits

2. Medium Priority:

   - Enhance error messages with field-specific information
   - Update documentation with clear examples
   - Add comprehensive test coverage

3. Low Priority:
   - Add migration guide
   - Enhance error reporting with more context
   - Add configuration options for SQL-specific limits
