# Bruno Environment Setup

Use this guide to configure local environment variables for the Bruno collection.

## File Location

Create a file at:

`bruno/.env`

## Required Variables

Add the following keys:

- `client_id`
- `client_secret`
- `scope`

Example:

```env
client_id=your-client-id
client_secret=your-client-secret
scope=your-api-scope
```

## Security Notes

- Never commit real secrets to source control.
- Use environment-specific credentials for local testing.
- Rotate secrets if they are accidentally exposed.

## Validation Checklist

1. `bruno/.env` exists locally.
2. All required keys are present and non-empty.
3. Authenticated Bruno requests return expected status codes.
