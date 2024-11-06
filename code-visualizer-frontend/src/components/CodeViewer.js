import React, { useState, useEffect } from 'react';
import axios from 'axios';

function CodeViewer() {
    const [repoUrl, setRepoUrl] = useState('');
    const [files, setFiles] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    const fetchFiles = async () => {
        setLoading(true);
        setError('');

        try {
            const response = await axios.get(`http://localhost:5018/api/code/list-files?repoUrl=${encodeURIComponent(repoUrl)}`);
            setFiles(response.data);
        } catch (err) {
            setError('Failed to fetch files. Please check the repository URL or try again later.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
            <h1>GitHub Code Visualizer</h1>
            <input
                type="text"
                placeholder="Enter GitHub repository URL"
                value={repoUrl}
                onChange={(e) => setRepoUrl(e.target.value)}
            />
            <button onClick={fetchFiles}>Fetch Files</button>

            {loading && <p>Loading...</p>}
            {error && <p style={{ color: 'red' }}>{error}</p>}

            <ul>
                {files.map((file, index) => (
                    <li key={index}>
                        {file.type === 'file' ? (
                            <a href={file.download_url} target="_blank" rel="noopener noreferrer">
                                {file.name}
                            </a>
                        ) : (
                            <strong>{file.name}</strong>
                        )}
                    </li>
                ))}
            </ul>
        </div>
    );
}

export default CodeViewer;