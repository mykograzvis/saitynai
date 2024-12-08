import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import { operationsService } from '../services/operationsService';
import 'bootstrap/dist/css/bootstrap.min.css';

const OperationsPage = () => {
  const [operations, setOperations] = useState([]);
  const [form, setForm] = useState({ name: '', description: '' });
  const [editingId, setEditingId] = useState(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const authToken = localStorage.getItem('authToken');
  const username = localStorage.getItem('username') || ''; // Get username from localStorage
  const { departmentId, doctorId } = useParams();
  const navigate = useNavigate(); // Initialize navigate function

  useEffect(() => {
    const authToken = localStorage.getItem('authToken');
    if (!authToken) {
      navigate('/');
    }
  }, [navigate]);

  const isAdmin = username === 'admin';

  useEffect(() => {
    const fetchOperations = async () => {
      try {
        setLoading(true);
        console.log(departmentId, doctorId);
        const data = await operationsService.getOperations(departmentId, doctorId);
        setOperations(data);
      } catch (err) {
        console.error('Error loading operations:', err);
        setError('Failed to load operations.');
      } finally {
        setLoading(false);
      }
    };
    fetchOperations();
  }, [departmentId, doctorId]);

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });
  };

  const handleCreateOrUpdate = async (e) => {
    e.preventDefault();
    try {
      if (editingId) {
        // Update operation
        await operationsService.updateOperation(departmentId, doctorId, editingId, form, authToken);
        setOperations((prev) =>
          prev.map((op) => (op.id === editingId ? { ...op, ...form } : op))
        );
        setEditingId(null);
      } else {
        // Create operation
        const newOperation = await operationsService.createOperation(departmentId, doctorId, form, authToken);
        setOperations((prev) => [...prev, newOperation]);
      }
      setForm({ name: '', description: '' }); // Reset form
    } catch (err) {
      console.error('Error creating/updating operation:', err);
      setError('Failed to save operation.');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this operation?')) {
      try {
        await operationsService.deleteOperation(departmentId, doctorId, id, authToken);
        setOperations((prev) => prev.filter((op) => op.id !== id));
      } catch (err) {
        console.error('Error deleting operation:', err);
        setError('Failed to delete operation.');
      }
    }
  };

  const handleEdit = (operation) => {
    setForm({ name: operation.name, description: operation.description });
    setEditingId(operation.id);
  };

  return (
    <div className="container mt-4">
      <h1 className="text-center mb-4">Operations for Doctor</h1>

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
            {operations.length > 0 ? (
              <ul className="list-group">
                {operations.map((operation) => (
                  <li
                    key={operation.id}
                    className="list-group-item d-flex justify-content-between align-items-center"
                  >
                    <div>
                      <strong>{operation.name}</strong>: {operation.description}
                    </div>
                    {isAdmin && (
                      <div>
                        <button
                          className="btn btn-sm btn-outline-primary me-2"
                          onClick={() => handleEdit(operation)}
                        >
                          Edit
                        </button>
                        <button
                          className="btn btn-sm btn-outline-danger"
                          onClick={() => handleDelete(operation.id)}
                        >
                          Delete
                        </button>
                      </div>
                    )}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="text-center">No operations available for this doctor.</p>
            )}
          </div>

          {isAdmin && (
            <div className="col-md-4">
              <div className="card">
                <div className="card-body">
                  <h5 className="card-title text-center">
                    {editingId ? 'Edit Operation' : 'Add New Operation'}
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
                      <label htmlFor="description" className="form-label">
                        Description
                      </label>
                      <textarea
                        className="form-control"
                        id="description"
                        name="description"
                        value={form.description}
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

export default OperationsPage;
