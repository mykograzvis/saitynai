import React from 'react';

const ResizableImage = () => {
  return (
    <img
      src="https://img.freepik.com/free-vector/people-walking-sitting-hospital-building-city-clinic-glass-exterior-flat-vector-illustration-medical-help-emergency-architecture-healthcare-concept_74855-10130.jpg"
      alt="My Image"
      style={{
        width: '100%',
        height: 'auto',
        objectFit: 'cover',
        objectPosition: 'center'
      }}
    />
  );
};

export default ResizableImage;