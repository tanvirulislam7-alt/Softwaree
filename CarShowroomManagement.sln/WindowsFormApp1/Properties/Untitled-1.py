import random

def f(x):
    return -x**2 + 10*x

def local_beam_search (K=3, iterations =100, step_size=0.1):

states = [random.uniform(0, 10) for _ in range(K)]

for _ in range(interations):
    all_neighbors =[]

for state in states:
    neighbors= [state + step_size, state-step_size]
    all_neighbors.extend(neighbors)

#select best K states 
states =sorted(all_neighbors,key=f,reverse=True)[:K]

best =max (states,key=f)
return best,f(best)

# Run
result =local_beam_search()

print("local beam search result :",result)  