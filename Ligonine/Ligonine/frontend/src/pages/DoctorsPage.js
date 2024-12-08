import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import { doctorsService } from '../services/doctorsService';
import 'bootstrap/dist/css/bootstrap.min.css';

const DoctorsPage = () => {
  const [doctors, setDoctors] = useState([]);
  const [form, setForm] = useState({ name: '', age: '', bloodType: '' });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const authToken = localStorage.getItem('authToken');
  const username = localStorage.getItem('username') || ''; // Get username from localStorage
  const { departmentId } = useParams();
  const navigate = useNavigate(); // Initialize navigate function

  useEffect(() => {
    const authToken = localStorage.getItem('authToken');
    if (!authToken) {
      navigate('/');
    }
  }, [navigate]);

  const isAdmin = username === 'admin';

  useEffect(() => {
    const fetchDoctors = async () => {
      try {
        setLoading(true);
        const data = await doctorsService.getDoctors(departmentId);
        setDoctors(data);
      } catch (err) {
        console.error('Error loading doctors:', err);
        setError('Failed to load doctors.');
      } finally {
        setLoading(false);
      }
    };
    fetchDoctors();
  }, [departmentId]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleCreateOrUpdate = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        // Update doctor
        await doctorsService.updateDoctor(departmentId, editingId, form, authToken);
        setDoctors((prev) =>
          prev.map((doc) => (doc.id === editingId ? { ...doc, ...form } : doc))
        );
        setEditingId(null);
      } else {
        // Create doctor
        const newDoctor = await doctorsService.createDoctor(departmentId, form, authToken);
        setDoctors((prev) => [...prev, newDoctor]);
      }
      setForm({ name: '', age: '', bloodType: '' }); // Reset form
    } catch (err) {
      console.error('Error creating/updating doctor:', err);
      setError('Failed to save doctor.');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this doctor?')) {
      try {
        await doctorsService.deleteDoctor(departmentId, id, authToken);
        setDoctors((prev) => prev.filter((doc) => doc.id !== id));
      } catch (err) {
        console.error('Error deleting doctor:', err);
        setError('Failed to delete doctor.');
      }
    }
  };

  const handleEdit = (doctor) => {
    setForm({ name: doctor.name, age: doctor.age, bloodType: doctor.bloodType });
    setEditingId(doctor.id);
  };

  const viewOperations = (doctorId) => {
    navigate(`/departments/${departmentId}/doctors/${doctorId}/operations`);
  };

  return (
    <div className="container mt-4">
      <h1 className="text-center mb-4">Doctors in Department</h1>

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
            {doctors.length > 0 ? (
              <ul className="list-group">
                {doctors.map((doctor) => (
                  <li
                    key={doctor.id}
                    className="list-group-item d-flex justify-content-between align-items-center"
                  >
                    <div>
                      <strong>{doctor.name}</strong> (Age: {doctor.age}, Blood Type: {doctor.bloodType})
                    </div>
                    <div>
                      <button
                        className="btn btn-sm btn-outline-secondary me-2"
                        onClick={() => viewOperations(doctor.id)}
                      >
                        View Operations
                      </button>
                      {isAdmin && (
                        <>
                          <button
                            className="btn btn-sm btn-outline-primary me-2"
                            onClick={() => handleEdit(doctor)}
                          >
                            Edit
                          </button>
                          <button
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => handleDelete(doctor.id)}
                          >
                            Delete
                          </button>
                        </>
                      )}
                    </div>
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-center text-muted">No doctors found in this department.</p>
            )}
          </div>

          {isAdmin && (
            <div className="col-md-4">
              <div className="card">
                <div className="card-body">
                  <h5 className="card-title text-center">
                    {editingId ? 'Edit Doctor' : 'Add New Doctor'}
                  </h5>
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
                      <label htmlFor="age" className="form-label">
                        Age
                      </label>
                      <input
                        type="number"
                        className="form-control"
                        id="age"
                        name="age"
                        value={form.age}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <div className="mb-3">
                      <label htmlFor="bloodType" className="form-label">
                        Blood Type
                      </label>
                      <input
                        type="text"
                        className="form-control"
                        id="bloodType"
                        name="bloodType"
                        value={form.bloodType}
                        onChange={handleInputChange}
                        required
                      />
                    </div>
                    <button type="submit" className="btn btn-primary w-100">
                      {editingId ? 'Update' : 'Create'}
                    </button>
                    {editingId && (
                      <button
                        type="button"
                        className="btn btn-secondary w-100 mt-2"
                        onClick={() => setEditingId(null)}
                      >
                        Cancel
                      </button>
                    )}
                  </form>
                </div>
              </div>
            </div>
          )}
        </div>
      )}
    </div>
  );
};

export default DoctorsPage;
