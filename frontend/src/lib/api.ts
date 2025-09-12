import axios from 'axios';

// Types matching the backend DTOs
export interface FileItemDto {
  id: number;
  fileName: string;
  contentType: string;
  fileSize: number;
  createdAt: string;
  updatedAt: string;
  parentFolderId?: number;
  isFolder: boolean;
}

export interface CreateFolderDto {
  folderName: string;
  parentFolderId?: number;
}

export interface MoveFileDto {
  fileId: number;
  newParentFolderId?: number;
}

export interface RenameFileDto {
  fileId: number;
  newName: string;
}

export interface RegisterDto {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export interface LoginDto {
  email: string;
  password: string;
}

export interface AuthResponseDto {
  token: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
}

// Configure axios base URL
const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'https://localhost:7144/api';

const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add auth token to requests
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('authToken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Auth API
export const authApi = {
  register: async (data: RegisterDto): Promise<AuthResponseDto> => {
    const response = await api.post('/auth/register', data);
    return response.data;
  },

  login: async (data: LoginDto): Promise<AuthResponseDto> => {
    const response = await api.post('/auth/login', data);
    return response.data;
  },
};

// Files API
export const filesApi = {
  getFiles: async (parentFolderId?: number): Promise<FileItemDto[]> => {
    const response = await api.get('/files', {
      params: { parentFolderId },
    });
    return response.data;
  },

  getFile: async (id: number): Promise<FileItemDto> => {
    const response = await api.get(`/files/${id}`);
    return response.data;
  },

  uploadFile: async (file: File, parentFolderId?: number): Promise<FileItemDto> => {
    const formData = new FormData();
    formData.append('file', file);
    if (parentFolderId) {
      formData.append('parentFolderId', parentFolderId.toString());
    }

    const response = await api.post('/files/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
    return response.data;
  },

  createFolder: async (data: CreateFolderDto): Promise<FileItemDto> => {
    const response = await api.post('/files/create-folder', data);
    return response.data;
  },

  downloadFile: async (id: number): Promise<Blob> => {
    const response = await api.get(`/files/${id}/download`, {
      responseType: 'blob',
    });
    return response.data;
  },

  moveFile: async (data: MoveFileDto): Promise<FileItemDto> => {
    const response = await api.put('/files/move', data);
    return response.data;
  },

  renameFile: async (data: RenameFileDto): Promise<FileItemDto> => {
    const response = await api.put('/files/rename', data);
    return response.data;
  },

  deleteFile: async (id: number): Promise<void> => {
    await api.delete(`/files/${id}`);
  },
};

export default api;