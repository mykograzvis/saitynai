import axios from 'axios';

const API_URL = 'https://localhost:7199/api/departments';

const getDepartments = async () => {
    try {
      const response = await axios.get(API_URL, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching departments:', error.response?.data || error.message);
      throw error;
    }
  };
  
  const createDepartment = async (departmentData) => {
    try {
      const response = await axios.post(API_URL, departmentData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error creating department:', error.response?.data || error.message);
      throw error;
    }
  };
  
  const updateDepartment = async (id, departmentData) => {
    try {
      const response = await axios.put(`${API_URL}/${id}`, departmentData, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Error updating department:', error.response?.data || error.message);
      throw error;
    }
  };
  
  const deleteDepartment = async (id) => {
    try {
      const response = await axios.delete(`${API_URL}/${id}`, {
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('authToken')}`
        }
      });
      return response.status === 204 ? 'Department deleted successfully' : 'Failed to delete department';
    } catch (error) {
      console.error('Error deleting department:', error.response?.data || error.message);
      throw error;
    }
  };
  
  export const departmentService = {
    getDepartments,
    createDepartment,
    updateDepartment,
    deleteDepartment
  };
  