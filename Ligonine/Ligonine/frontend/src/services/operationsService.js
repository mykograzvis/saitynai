import axios from 'axios';

const API_URL = 'https://ligonine-emcvhrfngsdheshq.canadacentral-01.azurewebsites.net/api/departments';

export const operationsService = {
  async getOperations(departmentId, doctorId) {
    const response = await axios.get(`${API_URL}/${departmentId}/doctors/${doctorId}/operations`);
    return response.data;
  },

  async getOperation(departmentId, doctorId, operationId) {
    const response = await axios.get(
      `${API_URL}/${departmentId}/doctors/${doctorId}/operations/${operationId}`
    );
    return response.data;
  },

  async createOperation(departmentId, doctorId, operationData, authToken) {
    const response = await axios.post(
      `${API_URL}/${departmentId}/doctors/${doctorId}/operations`,
      operationData,
      {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      }
    );
    return response.data;
  },

  async updateOperation(departmentId, doctorId, operationId, operationData, authToken) {
    await axios.put(
      `${API_URL}/${departmentId}/doctors/${doctorId}/operations/${operationId}`,
      operationData,
      {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      }
    );
  },

  async deleteOperation(departmentId, doctorId, operationId, authToken) {
    await axios.delete(
      `${API_URL}/${departmentId}/doctors/${doctorId}/operations/${operationId}`,
      {
        headers: {
          Authorization: `Bearer ${authToken}`,
        },
      }
    );
  },
};
