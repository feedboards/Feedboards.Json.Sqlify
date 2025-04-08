# Feedboards.Json.Sqlify v1.1.0 - Enhanced JSON Analysis & Type Support

This release introduces enhanced and improved JSON analysis handling to provide SQL generation.

### Changes

- Deleted `maxDepth` argument and errors code for SQL (`SQL_001`) and JSON (`JSN_002`) nesting limits
- Added support of new types such as `Tuple`, `Array` and updated `Nested` type
- Updated the JSON analysis, now this take all arrays and sum up them.
- Improved recursive handling of nested structures

### Breaking Changes

- **JSON Array Analysis**: The library now analyzes all elements in arrays to determine the schema, rather than using only the first element. This provides more accurate type detection but may result in different table structures for the same JSON data.
- **Removed Depth Limiting**: The `maxDepth` parameter has been removed from all methods. This means the library will now process nested structures of any depth without limitation.
- **Error Code Changes**: Error codes `SQL_001` and `JSN_002` related to nesting depth limits have been removed. Users relying on these specific error codes will need to update their error handling logic.
- **Type Detection**: The type detection algorithm has been updated to handle more complex types. This may result in different SQL types being generated for the same JSON data.

### Documentation Updates

- Updated error documentation with new error codes
- Added documentation for old versions

### Support

For support, please:

1. Check the documentation
2. Open an issue in the GitHub repository
3. Contact the maintainers

### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
