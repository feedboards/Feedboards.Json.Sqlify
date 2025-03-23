# TODO List

## Exception Usage Analysis

### SQL_001 Error Code Issues

1. The `SQL_001` error code is defined but not properly implemented:

   - ✓ Defined in `ErrorCodes.cs` as `NestedStructureLimit = "SQL_001"`
   - ✓ Documentation exists in both `errors_code.md` and `error_types.md`
   - ✓ Code now correctly uses `SQL_001` for SQL nesting limits

2. Current `NestedStructureLimitException` implementation:
   - ✓ Now uses correct error code pattern (`SQL_001` for SQL, `JSN_002` for JSON)
   - ✓ Added SQL-specific metadata fields (`TableName`, `NestedField`) in the ValidateSQLNesting method
   - ✓ Implemented distinction between JSON nesting limits and SQL nesting limits in validation logic

### Required Improvements

1. **ClickHouseSQLBuilder.cs**: [COMPLETED]

   - ✓ Added SQL nesting depth validation in `GenerateClickHouseSchema` method
   - ✓ Added tracking of nested field names for better error reporting
   - ✓ Now throws `NestedStructureLimitException` with `SQL_001` when SQL nesting exceeds limits

2. **NestedStructureLimitException.cs**: [COMPLETED]

   - ✓ Added support for `TableName` and `NestedField` metadata fields
   - ✓ Constructor properly handles SQL vs JSON error codes
   - ✓ Proper error code selection based on context implemented

3. **Test Coverage**: [COMPLETED]

   - ✓ Added tests specifically for SQL nesting limits
   - ✓ Test both unlimited depth (0 and negative values)
   - ✓ Test depth limit validation
   - ✓ Verify correct metadata is included in exceptions

4. **Documentation Updates**: [NOT STARTED]
   - ❌ Clarify the difference between JSON and SQL nesting limits
   - ❌ Update examples to show both types of nesting limit exceptions
   - ❌ Add migration guide for users of the current implementation

## Code Examples

### Suggested SQL Nesting Check Implementation [COMPLETED]

✓ Implemented in `ClickHouseSQLBuilder.cs` with the following improvements:

- Added validation for SQL nesting depth
- Added metadata for table name and nested field
- Added support for unlimited depth (0 or negative values)

### Suggested Exception Constructor [COMPLETED]

✓ Updated to properly handle SQL vs JSON error codes

## Priority Order

1. High Priority:

   - ✓ Implemented SQL nesting validation
   - ✓ Updated NestedStructureLimitException to support both error codes
   - ✓ Added basic test coverage for SQL nesting limits

2. Medium Priority:

   - ✓ Enhanced error messages with field-specific information
   - ❌ Update documentation with clear examples
   - ✓ Added comprehensive test coverage

3. Low Priority:
   - ❌ Add migration guide
   - ✓ Enhanced error reporting with more context

## Next Steps

1. ❌ Update documentation to reflect the changes
2. ❌ Create migration guide for users
