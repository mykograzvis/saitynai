import axios from 'axios';

const API_URL = 'https://ligonine-emcvhrfngsdheshq.canadacentral-01.azurewebsites.net/api/departments';

export const doctorsService = {
  async getDoctors(departmentId) {
    const response = await axios.get(`${API_URL}/${departmentId}/doctors`);
    return response.data;
  },

  async getDoctor(departmentId, doctorId) {
    const response = await axios.get(`${API_URL}/${departmentId}/doctors/${doctorId}`);
    return response.data;
  },

  async createDoctor(departmentId, doctorData, authToken) {
    const response = await axios.post(`${API_URL}/${departmentId}/doctors`, doctorData, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    });
    return response.data;
  },

  async updateDoctor(departmentId, doctorId, doctorData, authToken) {
    await axios.put(`${API_URL}/${departmentId}/doctors/${doctorId}`, doctorData, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    });
  },

  async deleteDoctor(departmentId, doctorId, authToken) {
    await axios.delete(`${API_URL}/${departmentId}/doctors/${doctorId}`, {
      headers: {
        Authorization: `Bearer ${authToken}`,
      },
    });
  },
};
