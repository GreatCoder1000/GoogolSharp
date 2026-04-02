import numpy as np
import matplotlib.pyplot as plt

# Your data (converted commas → dots)
x = np.array([
    0,
    0.0076,
    0.0141,
    0.025,
    0.0375,
    0.047,
    0.068,
    0.09,
    0.1143,
    0.1144,
    0.13415,
    0.1694,
    0.23016,
    0.30103,
    0.3619,
    0.47712,
    0.60206,
    0.69897,
    0.8451,
    0.90309,
    0.95424,
    1.0
])

y = np.array([
    0,
    0.0167,
    0.0311,
    0.0548,
    0.0815,
    0.1016,
    0.1445,
    0.1871,
    0.2309,
    0.2317,
    0.264,
    0.3167,
    0.3907,
    0.4553,
    0.5515,
    0.6563,
    0.7676,
    0.839,
    0.9269,
    0.9564,
    0.9802,
    1.0
])

# Fit a polynomial
coeffs = np.polyfit(x, y, 12)

print("Polynomial coefficients (highest degree first):")
print(coeffs)

# Create a polynomial function you can evaluate
p = np.poly1d(coeffs)

for i in 0,0.1144,0.30103,0.47712,0.60206,0.77815,1:
    print(f"p({i}) = {p(i)}")
    
    xx = np.linspace(0, 1, 500)
yy = p(xx)

# Plot the regression curve
plt.plot(xx, yy, label="Polynomial Fit", linewidth=2)

# Plot your original data points
plt.scatter(x, y, color="red", s=40, label="Data Points")

# Labels and styling
plt.title("12th degree Regression Fit")
plt.xlabel("x")
plt.ylabel("y")
plt.grid(True, alpha=0.3)
plt.legend()

plt.show()
