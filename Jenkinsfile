pipeline {
    agent any

    tools {
        jdk 'JDK'
        sonarRunner 'SonarScanner'
    }

    stages {

        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Build') {
            steps {
                echo "No build needed"
            }
        }

        stage('SonarQube Analysis') {
            steps {
                withSonarQubeEnv('SonarQube') {
                    bat '''
                    sonar-scanner ^
                    -Dsonar.projectKey=PhotographyPortfolio ^
                    -Dsonar.sources=. ^
                    -Dsonar.host.url=http://localhost:9000
                    '''
                }
            }
        }
    }
}
