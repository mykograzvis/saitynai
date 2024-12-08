import React, { useState, useEffect } from 'react';
import { departmentService } from '../services/departmentService';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const DepartmentsPage = () => {
  const [departments, setDepartments] = useState([]);
  const [form, setForm] = useState({ name: '', description: '' });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const authToken = localStorage.getItem('authToken');
    if (!authToken) {
        navigate('/');
    }
  }, [navigate]);
  
  const username = localStorage.getItem('username') || '';

  useEffect(() => {
    const fetchDepartments = async () => {
      try {
        setLoading(true);
        const data = await departmentService.getDepartments();
        setDepartments(data);
      } catch (err) {
        console.error('Error loading departments:', err);
        setError('Failed to load departments.');
      } finally {
        setLoading(false);
      }
    };
    fetchDepartments();
  }, []);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleCreateOrUpdate = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        await departmentService.updateDepartment(editingId, form);
        setDepartments((prev) =>
          prev.map((dept) => (dept.id === editingId ? { ...dept, ...form } : dept))
        );
        setEditingId(null);
      } else {
        const newDepartment = await departmentService.createDepartment(form);
        setDepartments((prev) => [...prev, newDepartment]);
      }
      setForm({ name: '', description: '' });
    } catch (err) {
      console.error('Error creating/updating department:', err);
      setError('Failed to save department.');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this department?')) {
      try {
        await departmentService.deleteDepartment(id);
        setDepartments((prev) => prev.filter((dept) => dept.id !== id));
      } catch (err) {
        console.error('Error deleting department:', err);
        setError('Failed to delete department.');
      }
    }
  };

  const handleEdit = (department) => {
    setForm({ name: department.name, description: department.description });
    setEditingId(department.id);
  };

  const navigateToDoctors = (departmentId) => {
    navigate(`/departments/${departmentId}/doctors`);
  };

  return (
    <div className="container mt-4">
      <h1 className="text-center mb-4">Departments</h1>

      {error && <div className="alert alert-danger">{error}</div>}

      {loading ? (
        <div className="text-center">
          <div className="spinner-border text-primary" role="status">
            <span className="visually-hidden">Loading...</span>
          </div>
        </div>
      ) : (
        <div className="row">
          <div className="col-md-8">
            <ul className="list-group">
              {departments.map((department) => (
                <li
                  key={department.id}
                  className="list-group-item d-flex justify-content-between align-items-center"
                >
                  <div>
                    <strong>{department.name}</strong>
                    <p className="mb-0">{department.description}</p>
                  </div>
                  <div>
                    {/* Conditionally render buttons based on admin nickname */}
                    {username === 'admin' && (
                      <>
                        <button
                          className="btn btn-sm btn-outline-primary me-2"
                          onClick={() => handleEdit(department)}
                        >
                          Edit
                        </button>
                        <button
                          className="btn btn-sm btn-outline-danger me-2"
                          onClick={() => handleDelete(department.id)}
                        >
                          Delete
                        </button>
                      </>
                    )}
                    <button
                      className="btn btn-sm btn-outline-success"
                      onClick={() => navigateToDoctors(department.id)}
                    >
                      See Doctors
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          </div>

          <div className="col-md-4">
            {username === 'admin' && (
              <div className="card">
                <div className="card-body">
                  <h5 className="card-title">{editingId ? 'Edit Department' : 'Add New Department'}</h5>
                  <form onSubmit={handleCreateOrUpdate}>
                    <div className="mb-3">
                      <label htmlFor="name" className="form-label">
                        Name
                      </label>
                      <input
                        type="text"
                        className="form-control"
                        id="name"
                        name="name"
                        value={form.name}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="mb-3">
                      <label htmlFor="description" className="form-label">
                        Description
                      </label>
                      <textarea
                        className="form-control"
                        id="description"
                        name="description"
                        rows="3"
                        value={form.description}
                        onChange={handleInputChange}
                        required
                      ></textarea>
                    </div>
                    <button type="submit" className="btn btn-primary w-100">
                      {editingId ? 'Update' : 'Create'}
                    </button>
                    {editingId && (
                      <button
                        type="button"
                        className="btn btn-secondary mt-2 w-100"
                        onClick={() => {
                          setEditingId(null);
                          setForm({ name: '', description: '' });
                        }}
                      >
                        Cancel
                      </button>
                    )}
                  </form>
                </div>
              </div>
            )}
          </div>
        </div>
      )}
    </div>
  );
};

export default DepartmentsPage;
