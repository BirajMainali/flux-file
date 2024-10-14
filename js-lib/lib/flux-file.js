/**
 * A custom hook for handling chunked file uploads.
 * 
 * @param {Object} options - Configuration options for the upload process.
 * @param {number} [options.chunkSize=1024 * 1024] - Size of each chunk in bytes (default is 1 MB).
 * @param {function} [options.uploadDelay] - Function that returns a promise for delaying between chunk uploads (default is a 30-second delay).
 * @param {function} options.initializeUpload - Function to initialize the upload. This will receive the file name and should return a unique file name.
 * @param {function} options.uploadChunk - Function to handle the actual upload of each chunk. Receives the unique file name, the chunk data, and its index.
 * @param {function} options.finalizeUpload - Function to call when the upload is complete. Receives the unique file name.
 * @param {function} options.reportProgress - Function to call to report progress. Receives a percentage of completion.
 * @param {function} options.handleError - Function to call when an error occurs during upload.
 * 
 * @returns {Object} An object containing the `startUpload` function for uploading files.
 */
const useChunkedFileUpload = (options = {}) => {
    const defaultOptions = {
        chunkSize: 1024 * 1024,
        uploadDelay: () => new Promise(resolve => setTimeout(resolve, 1000)),
        initializeUpload: (fileName) => { },
        uploadChunk: (uniqueFileName, chunkData, chunkIndex) => { },
        finalizeUpload: (uniqueFileName) => { },
        reportProgress: (progress) => { },
        handleError: (error) => { }
    };

    const config = { ...defaultOptions, ...options };

    /**
     * Uploads a file in chunks.
     * 
     * @param {File} file - The file object to be uploaded.
     * 
     * @returns {Promise<void>} A promise that resolves when the upload is complete or rejects on error.
     */
    const startUpload = async (file) => {
        try {
            const originalFileName = file.name;
            const uniqueFileName = await config.initializeUpload(originalFileName);
            const { chunkSize, uploadDelay, uploadChunk, reportProgress, finalizeUpload } = config;
            const totalChunks = Math.ceil(file.size / chunkSize);

            for (let chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++) {
                const startByte = chunkIndex * chunkSize;
                const endByte = Math.min(startByte + chunkSize, file.size);
                const chunkData = file.slice(startByte, endByte);
                await uploadChunk(uniqueFileName, chunkData, chunkIndex);
                reportProgress(((chunkIndex + 1) / totalChunks) * 100);
                await uploadDelay();
            }

            finalizeUpload(uniqueFileName);
        } catch (error) {
            config.handleError(error);
        }
    };

    return {
        startUpload
    };
};
