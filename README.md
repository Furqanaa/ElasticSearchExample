# ElasticSearchExample
a simple bookstore website. This service relies on an integration between relational database and elasticsearch. 

# Security rules related to uploading files to the server:
- Use a safe file name determined by the app. Don't use a file name provided by the user or the untrusted file name of the uploaded file.
  HTML encode the untrusted file name when displaying it. For example, logging the file name or displaying in UI (Razor automatically HTML encodes output).
- Allow only approved file extensions for the app's design specification.
- Check the size of an uploaded file. Set a maximum size limit to prevent large uploads.
- Upload files to a dedicated file upload area, preferably to a non-system drive. A dedicated location makes it easier to impose security restrictions on uploaded files. Disable execute permissions on the file upload location.
- Do not persist uploaded files in the same directory tree as the app (this actully can be done fine until you try to load an image file into a <img> html element, the html will refuse showing any image outside of the project directory for security reasons).
- Verify that client-side checks are performed on the server. Client-side checks are easy to circumvent.
- When files shouldn't be overwritten by an uploaded file with the same name, check the file name against the database or physical storage before uploading the file.
- Run a virus/malware scanner on uploaded content before the file is stored.

