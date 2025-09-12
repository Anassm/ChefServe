'use client';

import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { filesApi, FileItemDto } from '@/lib/api';
import { formatFileSize, formatDate, downloadFile } from '@/lib/utils';
import { 
  Folder, 
  File, 
  Upload, 
  FolderPlus, 
  Download, 
  Trash2, 
  Edit, 
  ArrowLeft 
} from 'lucide-react';

interface FileExplorerProps {
  currentFolderId?: number;
  onFolderChange?: (folderId?: number) => void;
}

export default function FileExplorer({ currentFolderId, onFolderChange }: FileExplorerProps) {
  const queryClient = useQueryClient();
  const [selectedFiles, setSelectedFiles] = useState<Set<number>>(new Set());
  const [showCreateFolder, setShowCreateFolder] = useState(false);
  const [newFolderName, setNewFolderName] = useState('');
  const [editingFile, setEditingFile] = useState<number | null>(null);
  const [editingName, setEditingName] = useState('');

  // Fetch files
  const { data: files = [], isLoading, error } = useQuery({
    queryKey: ['files', currentFolderId],
    queryFn: () => filesApi.getFiles(currentFolderId),
  });

  // Upload file mutation
  const uploadMutation = useMutation({
    mutationFn: (data: { file: File; parentFolderId?: number }) =>
      filesApi.uploadFile(data.file, data.parentFolderId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['files', currentFolderId] });
    },
  });

  // Create folder mutation
  const createFolderMutation = useMutation({
    mutationFn: (data: { folderName: string; parentFolderId?: number }) =>
      filesApi.createFolder(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['files', currentFolderId] });
      setShowCreateFolder(false);
      setNewFolderName('');
    },
  });

  // Delete file mutation
  const deleteMutation = useMutation({
    mutationFn: (id: number) => filesApi.deleteFile(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['files', currentFolderId] });
      setSelectedFiles(new Set());
    },
  });

  // Rename file mutation
  const renameMutation = useMutation({
    mutationFn: (data: { fileId: number; newName: string }) =>
      filesApi.renameFile(data),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['files', currentFolderId] });
      setEditingFile(null);
      setEditingName('');
    },
  });

  // Download file mutation
  const downloadMutation = useMutation({
    mutationFn: (id: number) => filesApi.downloadFile(id),
    onSuccess: (blob, id) => {
      const file = files.find(f => f.id === id);
      if (file) {
        downloadFile(blob, file.fileName);
      }
    },
  });

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      uploadMutation.mutate({ file, parentFolderId: currentFolderId });
    }
  };

  const handleCreateFolder = () => {
    if (newFolderName.trim()) {
      createFolderMutation.mutate({
        folderName: newFolderName.trim(),
        parentFolderId: currentFolderId,
      });
    }
  };

  const handleDoubleClick = (file: FileItemDto) => {
    if (file.isFolder) {
      onFolderChange?.(file.id);
    } else {
      downloadMutation.mutate(file.id);
    }
  };

  const handleDelete = (id: number) => {
    if (confirm('Are you sure you want to delete this item?')) {
      deleteMutation.mutate(id);
    }
  };

  const handleRename = (file: FileItemDto) => {
    setEditingFile(file.id);
    setEditingName(file.fileName);
  };

  const submitRename = () => {
    if (editingFile && editingName.trim()) {
      renameMutation.mutate({
        fileId: editingFile,
        newName: editingName.trim(),
      });
    }
  };

  const toggleSelection = (fileId: number) => {
    const newSelection = new Set(selectedFiles);
    if (newSelection.has(fileId)) {
      newSelection.delete(fileId);
    } else {
      newSelection.add(fileId);
    }
    setSelectedFiles(newSelection);
  };

  if (isLoading) {
    return <div className="flex justify-center p-8">Loading...</div>;
  }

  if (error) {
    return <div className="text-red-600 p-8">Error loading files</div>;
  }

  return (
    <div className="h-full flex flex-col">
      {/* Toolbar */}
      <div className="border-b bg-gray-50 p-4">
        <div className="flex items-center gap-4 mb-4">
          {currentFolderId && (
            <button
              onClick={() => onFolderChange?.(undefined)}
              className="flex items-center gap-2 text-blue-600 hover:text-blue-700"
            >
              <ArrowLeft className="w-4 h-4" />
              Back
            </button>
          )}
          <h2 className="text-xl font-semibold">
            {currentFolderId ? 'Folder' : 'My Files'}
          </h2>
        </div>

        <div className="flex items-center gap-2">
          <input
            type="file"
            id="file-upload"
            className="hidden"
            onChange={handleFileUpload}
            disabled={uploadMutation.isPending}
          />
          <label
            htmlFor="file-upload"
            className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 cursor-pointer"
          >
            <Upload className="w-4 h-4" />
            Upload File
          </label>

          <button
            onClick={() => setShowCreateFolder(true)}
            className="inline-flex items-center gap-2 px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700"
          >
            <FolderPlus className="w-4 h-4" />
            New Folder
          </button>

          {selectedFiles.size > 0 && (
            <button
              onClick={() => {
                selectedFiles.forEach(id => handleDelete(id));
              }}
              className="inline-flex items-center gap-2 px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
            >
              <Trash2 className="w-4 h-4" />
              Delete Selected ({selectedFiles.size})
            </button>
          )}
        </div>

        {/* Create folder input */}
        {showCreateFolder && (
          <div className="mt-4 flex items-center gap-2">
            <input
              type="text"
              value={newFolderName}
              onChange={(e) => setNewFolderName(e.target.value)}
              placeholder="Folder name"
              className="px-3 py-2 border border-gray-300 rounded"
              onKeyPress={(e) => e.key === 'Enter' && handleCreateFolder()}
            />
            <button
              onClick={handleCreateFolder}
              disabled={!newFolderName.trim() || createFolderMutation.isPending}
              className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
            >
              Create
            </button>
            <button
              onClick={() => {
                setShowCreateFolder(false);
                setNewFolderName('');
              }}
              className="px-4 py-2 bg-gray-600 text-white rounded hover:bg-gray-700"
            >
              Cancel
            </button>
          </div>
        )}
      </div>

      {/* File list */}
      <div className="flex-1 overflow-auto">
        {files.length === 0 ? (
          <div className="flex flex-col items-center justify-center h-full text-gray-500">
            <Folder className="w-16 h-16 mb-4" />
            <p>This folder is empty</p>
            <p className="text-sm">Upload files or create folders to get started</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 p-4">
            {files.map((file) => (
              <div
                key={file.id}
                className={`border rounded-lg p-4 hover:shadow-md cursor-pointer ${
                  selectedFiles.has(file.id) ? 'bg-blue-50 border-blue-300' : 'bg-white'
                }`}
                onClick={(e) => {
                  if (e.ctrlKey || e.metaKey) {
                    toggleSelection(file.id);
                  } else {
                    setSelectedFiles(new Set([file.id]));
                  }
                }}
                onDoubleClick={() => handleDoubleClick(file)}
              >
                <div className="flex items-start justify-between mb-2">
                  <div className="flex items-center gap-2">
                    {file.isFolder ? (
                      <Folder className="w-8 h-8 text-blue-500" />
                    ) : (
                      <File className="w-8 h-8 text-gray-500" />
                    )}
                  </div>
                  
                  <div className="flex gap-1">
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        handleRename(file);
                      }}
                      className="p-1 text-gray-600 hover:text-blue-600"
                    >
                      <Edit className="w-4 h-4" />
                    </button>
                    
                    {!file.isFolder && (
                      <button
                        onClick={(e) => {
                          e.stopPropagation();
                          downloadMutation.mutate(file.id);
                        }}
                        className="p-1 text-gray-600 hover:text-green-600"
                      >
                        <Download className="w-4 h-4" />
                      </button>
                    )}
                    
                    <button
                      onClick={(e) => {
                        e.stopPropagation();
                        handleDelete(file.id);
                      }}
                      className="p-1 text-gray-600 hover:text-red-600"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>

                <div>
                  {editingFile === file.id ? (
                    <input
                      type="text"
                      value={editingName}
                      onChange={(e) => setEditingName(e.target.value)}
                      onBlur={submitRename}
                      onKeyPress={(e) => e.key === 'Enter' && submitRename()}
                      className="w-full p-1 border rounded"
                      autoFocus
                    />
                  ) : (
                    <h3 className="font-medium truncate" title={file.fileName}>
                      {file.fileName}
                    </h3>
                  )}
                  
                  <div className="text-sm text-gray-500 mt-1">
                    {!file.isFolder && <p>{formatFileSize(file.fileSize)}</p>}
                    <p>{formatDate(file.createdAt)}</p>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}